let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        comments: []
    },
    methods: {
        submit: function (e) {
            e.preventDefault();
            console.log(this.body, this.thread);
            axios.post(this.route, 
                {
                    body: this.body,
                    thread: Number(this.thread)
                })
                .then(_ => this.load())
                .catch(console.error) 
        },
        
        load: function () {
            axios.get(this.route)
                .then(res => {
                    this.comments = res.data;
                })
                .catch(console.error)
        }
    },
    
    mounted() {
        this.thread = document.getElementById('thread').dataset.thread;
        this.route = document.getElementById('route').dataset.route;
        this.load();
    }
});
