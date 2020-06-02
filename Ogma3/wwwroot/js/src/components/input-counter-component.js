Vue.component('input-counter', {
    props: {
        max: {
            type: Number,
            required: true
        },
        min: {
            type: Number,
            default: 0
        },
        label: {
            type: String,
            required: true
        },
        value: {
            type: String,
            default: ''
        },
        validateMsg: {
            type: String,
            default: null
        }
    },
    data: function () {
        return {
            text: this.value,
            name: this.label.replace(/\s+/g, '')
        }
    },
    computed: {
        countChars: function () {
            return this.text.length.toLocaleString();
        },
        validate: function () {
            return this.text.length >= this.min && this.text.length <= this.max
        },
        validationString: function () {
            return this.validateMsg
                .replace('{0}', this.label)
                .replace('{1}', this.max)
                .replace('{2}', this.min);
        }
    },
    template: `
        <div class="o-form-group">
            <label :for="name">{{label}}</label>
            <input :name="name"
                   :id="name" 
                   type="text" 
                   class="o-form-control active-border" 
                   v-model="text">
            <div class="counter" :class="validate ? '' : 'invalid'">
                <div class="o-progress-bar" :style="{ width: Math.min(100, 100 * (countChars / max)) + '%' }"></div>
                <span>{{countChars}}/{{max.toLocaleString()}}</span>
            </div>
            <span v-if="!validate && validateMsg">{{validationString}}</span>
        </div>
    `
});