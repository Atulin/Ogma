let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        csrf: null,
        
        comments: [],
        total: 0,
        
        page: null,
        perPage: null,
        highlight: null,
    },
    methods: {

        // Submit the comment, load comments again, clean textarea
        submit: function (e) {
            e.preventDefault();
            axios.post(this.route,
                {
                    body: this.body,
                    thread: Number(this.thread)
                },{
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(_ => {
                    this.load();
                    this.body = null;
                })
                .catch(console.error)
        },

        // Load comments for the thread
        load: function () {
            const params = {
                thread: this.thread,
                page: this.page,
                highlight: this.highlight
            }
            
            // TODO: Has to load a different page if `this.highlight` is not null
            
            axios.get(this.route, { params: params })
                .then(res => {
                    this.total = res.data.total;
                    this.page = res.data.page ?? this.page;
                    
                    this.comments = res.data.elements.map(
                        (val, key) => ({val, key: key + ((this.page - 1) * this.perPage)})
                    );
                    
                    if (this.highlight) {
                        Vue.nextTick(_ => comments_vue.changeHighlight())
                    } else {
                        this.navigateToPage();
                    }
                })
                .catch(console.error)
        },

        // Handle Enter key input
        enter: function(e) {
            if (e.ctrlKey) this.submit(e)
        },

        // Parse date
        date: function (dt) {
            return dayjs(dt).format('DD MMM YYYY, HH:mm');
        },
        
        prevPage: function () {
            this.page = Math.max(1, this.page - 1);
            this.navigateToPage();
            this.load();
        },
        
        nextPage: function () {
            this.page = Math.min(this.page + 1, Math.ceil(this.total / this.perPage));
            this.navigateToPage();
            this.load();
        },
        
        changePage: function (idx) {
            this.page = idx;
            this.navigateToPage();
            this.load();
        },
        
        changeHighlight: function(idx = null, e) {
            e.preventDefault();
            this.highlight = idx ?? this.highlight;
            document
                .getElementById(`comment-${this.highlight}`)
                .scrollIntoView({behavior: "smooth", block: "center", inline: "nearest"});
            history.replaceState(undefined, undefined, `#comment-${idx}`)
        },

        navigateToPage: function () {
            if (this.page > 1) {
                history.replaceState(null, null, `#page-${this.page}`)
            } else {
                history.replaceState(null, null, window.location.href.split('#')[0])
            }
            if (this.highlight) this.highlight = null;
        },
    },

    mounted() {
        this.thread = document.getElementById('thread').dataset.thread;
        this.route = document.getElementById('route').dataset.route;
        this.perPage = document.getElementById('per-page').dataset.count;
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
        
        let hash = window.location.hash.split('-');
        
        if (hash[0] === '#page' && hash[1]) {
            this.page = Math.max(1, Number(hash[1] ?? 1));
        } else if (hash[0] === '#comment' && hash[1]) {
            this.page = 1;
            this.highlight = Number(hash[1]);
        } else {
            this.page = 1;
            history.replaceState(undefined, undefined, "")
        }
        
        this.load(); 
    }
});