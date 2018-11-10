var AccrualCalculatorComponent = Vue.component('accrual-calculator-component', {
        template: '#accrual-calculator-component',
        created: function () {
            this.getAccrual(this, function () {
                alert('Error retrieving the accrual. Try again.');
            });
        },
        provide: function () {
            return {
                getAccrual: this.getAccrual,
                updateAccrual: this.updateAccrual,
                deleteAccrual: this.deleteAccrual,
                addAccrualAction: this.addAccrualAction,
                deleteAccrualAction: this.deleteAccrualAction,
                copyAccrual: this.copyAccrual
            };
        },
        computed: {
            accrual: function () {
                return this.$store.state.accrual;
            }
        },
        methods: {
            getAccrual: function (instance, errorCallback) {
                return this.$store.state.accrual;
            },
            copyAccrual: function () {
                var a = Object.assign({}, this.accrual);
                delete a.actions;
                delete a.rows;
                delete a.lastModified;
                delete a.accrualId;

                return a;
            },
            updateAccrual: function (mutated, successCallback, errorCallback) {
                var self = this;
                this.$store.commit('updateAccrual', mutated);
                if (successCallback) {
                    successCallback();
                }
            },
            deleteAccrual: function (successCallback, errorCallback) {
                var self = this;
            },
            addAccrualAction: function (adjustment, successCallback, errorCallback) {
                var self = this;
                this.$store.commit('addAccrualAction', adjustment);
                if (successCallback) {
                    successCallback();
                }
            },
            deleteAccrualAction: function (accrualActionId, errorCallback) {
                var self = this;
                this.$store.commit('deleteAccrualAction', accrualActionId);
            }
        }
    });

var DemoCalculatorComponent = Vue.component('demo-calculator-component', {
    template: '#demo-calculator-component',
    methods: {}
});


Vue.component('accrual-table-component', {
    template: '#accrual-table-component',
    props: ['accrual'],
    data: function () {
        return {};
    },
    methods: {}
});

Vue.component('accrual-row-component', {
    template: '#accrual-row-component',
    props: ['row', 'accrualRate'],
    methods: {}
});

Vue.component('actions-component', {
    template: '#actions-component',
    props: ['accrual'],
    inject: ['updateAccrual', 'deleteAccrual', 'copyAccrual'],
    data: function () {
        return {
            localAccrual: {}
        };
    },
    //        watch: {
    //            accrual: function(current, previous) {
    //                if (current && current.name) {
    //                    this.localAccrual = this.copyAccrual();
    //                }
    //            }
    //        },
    created: function () {
        this.localAccrual = this.copyAccrual();
    },
    methods: {
        updateAccrualClick: function (which) {
            var update = false;
            if (which === 'heart') {
                update = true;
                this.localAccrual.isHeart = !this.localAccrual.isHeart;
            } else if (which === 'archive') {
                update = true;
                this.localAccrual.isArchived = !this.localAccrual.isArchived;
            }

            if (update) {
                this.updateAccrual(this.localAccrual);
            }
        },
        deleteAccrualClick: function () {
            var answer = confirm('THIS ACTION IS PERMANENT!\nYOUR DATA WILL BE GONE FOREVER.\nClick OK to delete the accrual forever.\nClick Cancel to you know ... not do anything.');
            if (answer) {
                this.deleteAccrual(function () {
                    alert('The accrual has been permanently removed from our system.');
                }, function () {
                    alert('Error deleting the accrual. Check the console for additional information.');
                });
            }
        }
    }
});

Vue.component('cool-stats-component', {
    template: '#cool-stats-component',
    props: ['accrual'],
    inject: ['updateAccrual', 'copyAccrual'],
    data:
        function () {
            return {
                ui: {
                    isView: true,
                    ready: false
                },
                edit: {}
            };
        },
    methods: {
        doneClick: function () {
            var self = this;
            this.updateAccrual(this.edit, function () {
                self.edit = undefined;
                self.ui.isView = true;
            });
        },
        editClick: function () {
            this.edit = this.copyAccrual();
            this.ui.isView = false;
        },
        discardClick: function () {
            this.edit = undefined;
            this.ui.isView = true;
        }
    },
    computed: {
        isView: function () {
            return this.ui.isView;
        }
    },
    created: function () {
        // pause while we wait for the accrual prop to get populated
        // this approach with setTimeout is a previous approach i wanted to keep around for future reference
        // but for now i get the same effect using 'watch'
        //            this.waitForAccrual();
    }
});

Vue.component('action-history-component', {
    template: '#action-history-component',
    props: ['accrual'],
    methods: {}
});

Vue.component('action-history-detail-component', {
    template: '#action-history-detail-component',
    props: ['action'],
    inject: ['deleteAccrualAction'],
    methods: {
        deleteAccrualActionClick: function () {
            this.deleteAccrualAction(this.action.accrualActionId);
        }
    }
});

Vue.component('add-action-component', {
    template: '#add-action-component',
    props: ['accrual'],
    inject: ['addAccrualAction'],
    data: function () {
        return {
            ui: {
                show: false
            },
            adjustment: {}
        };
    },
    computed: {
        isDisabled: function () {
            // the method returns true if the button should be disabled
            // return false if all validations are successful

            if (this.adjustment.accrualAction !== 'ADJUSTMENT') {
                return true;
            }

            var amount = parseInt(this.adjustment.amount);
            if (!amount || amount < -40 || amount > 40) {
                return true;
            }

            var timestamp = Date.parse(this.adjustment.actionDate);
            if (isNaN(timestamp) === true) {
                return true;
            } else {
                if (new Date(this.adjustment.actionDate) < new Date(this.accrual.startingDate)) {
                    return true;
                }
            }

            if (this.adjustment.note && this.adjustment.note.length > 100) {
                return true;
            }

            return false;
        }
    },
    methods: {
        showAdjustmentPopupClick: function () {
            var timestamp = new Date();
            var initialActionDate = new Date(timestamp.getFullYear(), timestamp.getMonth(), timestamp.getDate());
            var offset = initialActionDate.getTimezoneOffset();
            var yourDate = new Date(initialActionDate.getTime() + (offset * 60 * 1000));
            var date = yourDate.toISOString().split('T')[0];

            this.adjustment.accrualAction = 'ADJUSTMENT';
            this.adjustment.actionDate = date;
            this.ui.show = !this.ui.show;
        },
        addAccrualActionClick: function () {
            var self = this;
            this.addAccrualAction(this.adjustment, function () {
                self.adjustment = {};
                self.ui.show = false;
            });
        }
    }
});

var MongoStore = new Vuex.Store({
    state: {
        accrual: {
            name: "Accrual Calculator Demo",
            accrualRate: 7,
            hourlyRate: 55,
            minHours: 40,
            maxHours: 255,
            ending: "PLUSONE",
            dayOfPayA: 7,
            dayOfPayB: 21,
            accrualFrequency: "SEMIMONTHLY",
            startingDate: "2018-10-07",
            lastModified: "2018-11-07T07:00:00.000Z",
            actions: [
                {
                    accrualActionId: "9946e32a-00ac-4a3e-95d6-7a0999ee9118",
                    accrualAction: "CREATED",
                    actionDate: new Date(2018, 10, 31),
                    amount: null,
                    note: null,
                    dateCreated: new Date(2018, 10, 7)
                },
                {
                    accrualActionId: "91c66ded-bb04-47a4-97aa-1312d2b4c3ff",
                    accrualAction: "ADJUSTMENT",
                    actionDate: new Date(2018, 10, 7),
                    amount: -8,
                    note: "this is my day off",
                    dateCreated: new Date(2018, 10, 7)
                }],
            rows: [{ "rowId": 0, "currentAccrual": 22, "accrualDate": null, "hoursUsed": 0, "actions": [] }, { "rowId": 1, "currentAccrual": 29, "accrualDate": "2018-10-07", "hoursUsed": 0, "actions": [] }, { "rowId": 2, "currentAccrual": 36, "accrualDate": "2018-10-21", "hoursUsed": 0, "actions": [] }, { "rowId": 3, "currentAccrual": 43, "accrualDate": "2018-11-07", "hoursUsed": 0, "actions": [{ accrualActionId: "91c66ded-bb04-47a4-97aa-1312d2b4c3ff", accrualAction: "ADJUSTED", actionDate: "2018-10-31", amount: -8, note: "I sure love Halloween", dateCreated: new Date(2018, 10, 12) }] }, { "rowId": 4, "currentAccrual": 50, "accrualDate": "2018-11-21", "hoursUsed": 0, "actions": [] }, { "rowId": 5, "currentAccrual": 57, "accrualDate": "2018-12-07", "hoursUsed": 0, "actions": [] }, { "rowId": 6, "currentAccrual": 64, "accrualDate": "2018-12-21", "hoursUsed": 0, "actions": [] }, { "rowId": 7, "currentAccrual": 71, "accrualDate": "2019-01-07", "hoursUsed": 0, "actions": [] }, { "rowId": 8, "currentAccrual": 78, "accrualDate": "2019-01-21", "hoursUsed": 0, "actions": [] }, { "rowId": 9, "currentAccrual": 85, "accrualDate": "2019-02-07", "hoursUsed": 0, "actions": [] }, { "rowId": 10, "currentAccrual": 92, "accrualDate": "2019-02-21", "hoursUsed": 0, "actions": [] }, { "rowId": 11, "currentAccrual": 99, "accrualDate": "2019-03-07", "hoursUsed": 0, "actions": [] }, { "rowId": 12, "currentAccrual": 106, "accrualDate": "2019-03-21", "hoursUsed": 0, "actions": [] }, { "rowId": 13, "currentAccrual": 113, "accrualDate": "2019-04-07", "hoursUsed": 0, "actions": [] }, { "rowId": 14, "currentAccrual": 120, "accrualDate": "2019-04-21", "hoursUsed": 0, "actions": [] }, { "rowId": 15, "currentAccrual": 127, "accrualDate": "2019-05-07", "hoursUsed": 0, "actions": [] }, { "rowId": 16, "currentAccrual": 134, "accrualDate": "2019-05-21", "hoursUsed": 0, "actions": [] }, { "rowId": 17, "currentAccrual": 141, "accrualDate": "2019-06-07", "hoursUsed": 0, "actions": [] }, { "rowId": 18, "currentAccrual": 148, "accrualDate": "2019-06-21", "hoursUsed": 0, "actions": [] }, { "rowId": 19, "currentAccrual": 155, "accrualDate": "2019-07-07", "hoursUsed": 0, "actions": [] }, { "rowId": 20, "currentAccrual": 162, "accrualDate": "2019-07-21", "hoursUsed": 0, "actions": [] }, { "rowId": 21, "currentAccrual": 169, "accrualDate": "2019-08-07", "hoursUsed": 0, "actions": [] }, { "rowId": 22, "currentAccrual": 176, "accrualDate": "2019-08-21", "hoursUsed": 0, "actions": [] }, { "rowId": 23, "currentAccrual": 183, "accrualDate": "2019-09-07", "hoursUsed": 0, "actions": [] }, { "rowId": 24, "currentAccrual": 190, "accrualDate": "2019-09-21", "hoursUsed": 0, "actions": [] }, { "rowId": 25, "currentAccrual": 197, "accrualDate": "2019-10-07", "hoursUsed": 0, "actions": [] }, { "rowId": 26, "currentAccrual": 204, "accrualDate": "2019-10-21", "hoursUsed": 0, "actions": [] }, { "rowId": 27, "currentAccrual": 211, "accrualDate": "2019-11-07", "hoursUsed": 0, "actions": [] }, { "rowId": 28, "currentAccrual": 218, "accrualDate": "2019-11-21", "hoursUsed": 0, "actions": [] }, { "rowId": 29, "currentAccrual": 225, "accrualDate": "2019-12-07", "hoursUsed": 0, "actions": [] }, { "rowId": 30, "currentAccrual": 232, "accrualDate": "2019-12-21", "hoursUsed": 0, "actions": [] }]
        }
    },
    mutations: {
        updateAccrual: function (state, accrual) {
            Vue.set(state.accrual, 'name', accrual.name);
            Vue.set(state.accrual, 'minHours', accrual.minHours);
            Vue.set(state.accrual, 'maxHours', accrual.maxHours);
            Vue.set(state.accrual, 'startingDate', accrual.startingDate);
            Vue.set(state.accrual, 'isHeart', accrual.isHeart);
            Vue.set(state.accrual, 'isArchived', accrual.isArchived);
            Vue.set(state.accrual, 'startingDate', accrual.startingDate);
            Vue.set(state.accrual, 'ending', accrual.ending);
            Vue.set(state.accrual, 'accrualFrequency', accrual.accrualFrequency);
            Vue.set(state.accrual, 'hourlyRate', parseInt(accrual.hourlyRate));
            Vue.set(state.accrual, 'dayOfPayA', parseInt(accrual.dayOfPayA));
            Vue.set(state.accrual, 'dayOfPayB', parseInt(accrual.dayOfPayB));
            Vue.set(state.accrual, 'lastModified', new Date().toISOString());
        },
        addAccrualAction: function (state, action) {
            function guid() {
                function s4() {
                    return Math.floor((1 + Math.random()) * 0x10000)
                        .toString(16)
                        .substring(1);
                }
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
            }

            action.accrualActionId = guid();
            action.dateCreated = new Date().toISOString();

            var actions = state.accrual.actions.slice();
            actions.push(action);

            var rows = state.accrual.rows ? state.accrual.rows.slice() : [];

            var found = false;

            rows.forEach(function (row) {
                var date1 = new Date(row.accrualDate);
                var date2 = new Date(action.actionDate);
                if (!found && date2 <= date1) {
                    if (!row.actions) {
                        row.actions = [];
                    }
                    row.actions.push(action);
                    found = true;
                }
            });

            Vue.set(state.accrual, 'actions', actions);
            Vue.set(state.accrual, 'rows', rows);
        },
        deleteAccrualAction: function (state, accrualActionId) {
            var actions = state.accrual.actions.slice();
            for (var i = actions.length - 1; i >= 0; i--) {
                var action = actions[i];
                if (action.accrualActionId === accrualActionId) {
                    actions.splice(i, 1);
                }
            }

            var rows = state.accrual.rows.slice();
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];

                for (var j = row.actions ? row.actions.length - 1 : 0; j >= 0; j--) {
                    var action = row.actions[j];
                    if (action.accrualActionId === accrualActionId) {
                        row.actions.splice(j, 1);
                        break;
                    }
                }
            }

            Vue.set(state.accrual, 'actions', actions);
            Vue.set(state.accrual, 'rows', rows);
        }
    }
});

var DemoStore = new Vuex.Store({
    state: {
        accrual: {
            name: "Accrual Calculator Demo",
            accrualRate: 7,
            hourlyRate: 55,
            minHours: 40,
            maxHours: 255,
            ending: "PLUSONE",
            dayOfPayA: 7,
            dayOfPayB: 21,
            accrualFrequency: "SEMIMONTHLY",
            startingDate: "2018-10-07",
            lastModified: "2018-11-07T07:00:00.000Z",
            actions: [
                {
                    accrualActionId: "9946e32a-00ac-4a3e-95d6-7a0999ee9118",
                    accrualAction: "CREATED",
                    actionDate: new Date(2018, 10, 31),
                    amount: null,
                    note: null,
                    dateCreated: new Date(2018, 10, 7)
                },
                {
                    accrualActionId: "91c66ded-bb04-47a4-97aa-1312d2b4c3ff",
                    accrualAction: "ADJUSTMENT",
                    actionDate: new Date(2018, 10, 7),
                    amount: -8,
                    note: "this is my day off",
                    dateCreated: new Date(2018, 10, 7)
                }],
            rows: [{ "rowId": 0, "currentAccrual": 22, "accrualDate": null, "hoursUsed": 0, "actions": [] }, { "rowId": 1, "currentAccrual": 29, "accrualDate": "2018-10-07", "hoursUsed": 0, "actions": [] }, { "rowId": 2, "currentAccrual": 36, "accrualDate": "2018-10-21", "hoursUsed": 0, "actions": [] }, { "rowId": 3, "currentAccrual": 43, "accrualDate": "2018-11-07", "hoursUsed": 0, "actions": [{ accrualActionId: "91c66ded-bb04-47a4-97aa-1312d2b4c3ff", accrualAction: "ADJUSTED", actionDate: "2018-10-31", amount: -8, note: "I sure love Halloween", dateCreated: new Date(2018, 10, 12) }] }, { "rowId": 4, "currentAccrual": 50, "accrualDate": "2018-11-21", "hoursUsed": 0, "actions": [] }, { "rowId": 5, "currentAccrual": 57, "accrualDate": "2018-12-07", "hoursUsed": 0, "actions": [] }, { "rowId": 6, "currentAccrual": 64, "accrualDate": "2018-12-21", "hoursUsed": 0, "actions": [] }, { "rowId": 7, "currentAccrual": 71, "accrualDate": "2019-01-07", "hoursUsed": 0, "actions": [] }, { "rowId": 8, "currentAccrual": 78, "accrualDate": "2019-01-21", "hoursUsed": 0, "actions": [] }, { "rowId": 9, "currentAccrual": 85, "accrualDate": "2019-02-07", "hoursUsed": 0, "actions": [] }, { "rowId": 10, "currentAccrual": 92, "accrualDate": "2019-02-21", "hoursUsed": 0, "actions": [] }, { "rowId": 11, "currentAccrual": 99, "accrualDate": "2019-03-07", "hoursUsed": 0, "actions": [] }, { "rowId": 12, "currentAccrual": 106, "accrualDate": "2019-03-21", "hoursUsed": 0, "actions": [] }, { "rowId": 13, "currentAccrual": 113, "accrualDate": "2019-04-07", "hoursUsed": 0, "actions": [] }, { "rowId": 14, "currentAccrual": 120, "accrualDate": "2019-04-21", "hoursUsed": 0, "actions": [] }, { "rowId": 15, "currentAccrual": 127, "accrualDate": "2019-05-07", "hoursUsed": 0, "actions": [] }, { "rowId": 16, "currentAccrual": 134, "accrualDate": "2019-05-21", "hoursUsed": 0, "actions": [] }, { "rowId": 17, "currentAccrual": 141, "accrualDate": "2019-06-07", "hoursUsed": 0, "actions": [] }, { "rowId": 18, "currentAccrual": 148, "accrualDate": "2019-06-21", "hoursUsed": 0, "actions": [] }, { "rowId": 19, "currentAccrual": 155, "accrualDate": "2019-07-07", "hoursUsed": 0, "actions": [] }, { "rowId": 20, "currentAccrual": 162, "accrualDate": "2019-07-21", "hoursUsed": 0, "actions": [] }, { "rowId": 21, "currentAccrual": 169, "accrualDate": "2019-08-07", "hoursUsed": 0, "actions": [] }, { "rowId": 22, "currentAccrual": 176, "accrualDate": "2019-08-21", "hoursUsed": 0, "actions": [] }, { "rowId": 23, "currentAccrual": 183, "accrualDate": "2019-09-07", "hoursUsed": 0, "actions": [] }, { "rowId": 24, "currentAccrual": 190, "accrualDate": "2019-09-21", "hoursUsed": 0, "actions": [] }, { "rowId": 25, "currentAccrual": 197, "accrualDate": "2019-10-07", "hoursUsed": 0, "actions": [] }, { "rowId": 26, "currentAccrual": 204, "accrualDate": "2019-10-21", "hoursUsed": 0, "actions": [] }, { "rowId": 27, "currentAccrual": 211, "accrualDate": "2019-11-07", "hoursUsed": 0, "actions": [] }, { "rowId": 28, "currentAccrual": 218, "accrualDate": "2019-11-21", "hoursUsed": 0, "actions": [] }, { "rowId": 29, "currentAccrual": 225, "accrualDate": "2019-12-07", "hoursUsed": 0, "actions": [] }, { "rowId": 30, "currentAccrual": 232, "accrualDate": "2019-12-21", "hoursUsed": 0, "actions": [] }]
        }
    },
    mutations: {
        updateAccrual: function (state, accrual) {
            Vue.set(state.accrual, 'name', accrual.name);
            Vue.set(state.accrual, 'minHours', accrual.minHours);
            Vue.set(state.accrual, 'maxHours', accrual.maxHours);
            Vue.set(state.accrual, 'startingDate', accrual.startingDate);
            Vue.set(state.accrual, 'isHeart', accrual.isHeart);
            Vue.set(state.accrual, 'isArchived', accrual.isArchived);
            Vue.set(state.accrual, 'startingDate', accrual.startingDate);
            Vue.set(state.accrual, 'ending', accrual.ending);
            Vue.set(state.accrual, 'accrualFrequency', accrual.accrualFrequency);
            Vue.set(state.accrual, 'hourlyRate', parseInt(accrual.hourlyRate));
            Vue.set(state.accrual, 'dayOfPayA', parseInt(accrual.dayOfPayA));
            Vue.set(state.accrual, 'dayOfPayB', parseInt(accrual.dayOfPayB));
            Vue.set(state.accrual, 'lastModified', new Date().toISOString());
        },
        addAccrualAction: function (state, action) {
            function guid() {
                function s4() {
                    return Math.floor((1 + Math.random()) * 0x10000)
                        .toString(16)
                        .substring(1);
                }
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
            }

            action.accrualActionId = guid();
            action.dateCreated = new Date().toISOString();

            var actions = state.accrual.actions.slice();
            actions.push(action);

            var rows = state.accrual.rows ? state.accrual.rows.slice() : [];

            var found = false;

            rows.forEach(function (row) {
                var date1 = new Date(row.accrualDate);
                var date2 = new Date(action.actionDate);
                if (!found && date2 <= date1) {
                    if (!row.actions) {
                        row.actions = [];
                    }
                    row.actions.push(action);
                    found = true;
                }
            });

            Vue.set(state.accrual, 'actions', actions);
            Vue.set(state.accrual, 'rows', rows);
        },
        deleteAccrualAction: function (state, accrualActionId) {
            var actions = state.accrual.actions.slice();
            for (var i = actions.length - 1; i >= 0; i--) {
                var action = actions[i];
                if (action.accrualActionId === accrualActionId) {
                    actions.splice(i, 1);
                }
            }

            var rows = state.accrual.rows.slice();
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];

                for (var j = row.actions ? row.actions.length - 1 : 0; j >= 0; j--) {
                    var action = row.actions[j];
                    if (action.accrualActionId === accrualActionId) {
                        row.actions.splice(j, 1);
                        break;
                    }
                }
            }

            Vue.set(state.accrual, 'actions', actions);
            Vue.set(state.accrual, 'rows', rows);
        }
    }
});