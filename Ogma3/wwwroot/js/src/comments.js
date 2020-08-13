let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        csrf: null,
        
        comments: [],
        visibleComments: [],
        
        page: 1,
        perPage: 10,
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
            axios.get(this.route, { params:{ thread:this.thread } })
                .then(res => {
                    this.comments = res.data.map(
                        (val, key) => ({val, key})
                    ).reverse();
                    
                    if (this.highlight) {
                        this.navigateToComment()
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
            this.navigateToPage()
        },
        
        nextPage: function () {
            this.page = Math.min(this.page + 1, Math.ceil(this.comments.length / this.perPage));
            this.navigateToPage()
        },
        
        changePage: function (idx) {
            this.page = idx;
            this.navigateToPage()
        },
        
        changeHighlight: function(idx = null) {
            this.highlight = idx ?? this.highlight;
            document
                .getElementById(`comment-${this.highlight}`)
                .scrollIntoView({behavior: "smooth", block: "center", inline: "nearest"});
        },

        navigateToPage: function () {
            this.visibleComments = this.comments.slice((this.page - 1) * this.perPage, this.page * this.perPage);
            window.location.hash = `page.${this.page}`;
            if (this.highlight) this.highlight = null;
        },
        
        navigateToComment: function () {
            let idx = this.comments.findIndex(e => e.key+1 === this.highlight);
            this.page = Math.floor(idx / this.perPage) + 1;
            this.visibleComments = this.comments.slice((this.page - 1) * this.perPage, this.page * this.perPage);
            Vue.nextTick(function () {
                comments_vue.changeHighlight();
            })
        }
    },

    mounted() {
        this.thread = document.getElementById('thread').dataset.thread;
        this.route = document.getElementById('route').dataset.route;
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
        
        let hash = window.location.hash.split('.');
        
        if (hash[0] === '#page' && hash[1]) {
            this.page = Math.max(1, Number(hash[1] ?? 1));
        } else if (hash[0] === '#comment' && hash[1]) {
            this.highlight = Number(hash[1]);
        } else {
            window.location.hash = null; 
        }
        
        this.load(); 
    }
});