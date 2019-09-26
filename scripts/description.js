"use strict";
const request = require("request");

module.exports = {
    name: "Starcounter 3.0",
    isStarcounter: true,
    server: {},
    host: "http://127.0.0.1:8080/starcounter-mmb",

    startup: function (host, cb) {
        const self = this;

        this.host = host;
        this.requestPool = { maxSockets: 32 };

        cb();
    },

    warmup: function (db, cb) {
        module.exports.getVersion(db, "", function (err, result) {
            if (err) return cb(err);

            console.log("INFO version " + JSON.stringify(result));
            console.log("INFO Running test benchmark");

            const timeout = 5000;
            const checkIsBenchmarking = function () {
                module.exports.starGetRequest("/benchmark/status", function (err, body) {
                    if (err) {
                        return cb(err);
                    }

                    var status = JSON.parse(body);

                    if (status.IsBenchmarking) {
                        console.log("INFO LastBenchmark: " + status.LastBenchmark);
                        setTimeout(checkIsBenchmarking, timeout);
                    } else {
                        console.log("INFO warmup has been succesfully completed");
                        return cb();
                    }
                });
            };

            module.exports.starGetRequest("/benchmark/All/28", function (err) {
                if (err) {
                    return cb(err);
                }

                setTimeout(checkIsBenchmarking, timeout);
            });
        });
    },

    starGetRequest: function (uri, cb) {
        const url = this.host + uri;

        request({
            url: url,
            pool: this.requestPool
        }, function (error, response, body) {
            if (error) {
                console.log(error);
                cb(error);
                return;
            }

            cb(null, body);
        });
    },

    starPostRequest: function (uri, data, cb) {
        const url = this.host + uri;

        request({
            url: url,
            method: "POST",
            json: data,
            pool: this.requestPool
        }, function (error, response, body) {
            if (error) {
                console.log(error);
                cb(error);
                return;
            }

            cb(null, body);
        });
    },

    starGetDescriptionRequest: function (uri, cb) {
        this.starGetRequest("/description" + uri, cb);
    },

    starPostDescriptionRequest: function (uri, data, cb) {
        this.starPostRequest("/description" + uri, data, cb);
    },

    getVersion: function (db, name, cb) {
        this.starGetDescriptionRequest("/get-version", cb);
    },

    loadRelations: function (db, name, cb) {
        this.starGetDescriptionRequest("/load-relations", cb);
    },

    getCollection: function (db, name, cb) {
        cb(null, name);
    },

    dropCollection: function (db, name, cb) {
        this.starGetDescriptionRequest("/drop-collection", cb);
    },

    createCollection: function (db, name, cb) {
        cb(null, name);
    },

    createCollectionSync: function (db, name, cb) {
        cb(null, name);
    },

    getDocument: function (db, table, id, cb) {
        const uri = "/get-document-p/" + id;
        this.starGetDescriptionRequest(uri, cb);
    },

    saveDocument: function (db, table, doc, cb) {
        this.starPostDescriptionRequest("/save-document", doc, cb);
    },

    saveDocumentSync: function (db, table, doc, cb) {
        this.starPostDescriptionRequest("/save-document-sync", doc, cb);
    },

    aggregate: function (db, table, cb) {
        this.starGetDescriptionRequest("/aggregate", cb);
    },

    neighbors: function (db, tableP, tableR, id, i, cb) {
        const uri = "/neighbors-p/" + id;
        this.starGetDescriptionRequest(uri, function (error, body) {
            cb(error, body / 1);
        });
    },

    neighbors2: function (db, tableP, tableR, id, i, cb) {
        const uri = "/neighbors-p-2/" + id;
        this.starGetDescriptionRequest(uri, function (error, body) {
            cb(error, body / 1);
        });
    },

    neighbors2data: function (db, tableP, tableR, id, i, cb) {
        const uri = "/neighbors-p-2-data/" + id;
        this.starGetDescriptionRequest(uri, function (error, body) {
            cb(error, body / 1);
        });
    },

    shortestPath: function (db, collP, collR, path, i, cb) {
        const uri = "/shortest-path-p/" + path.from + "/" + path.to;
        this.starGetDescriptionRequest(uri, function (error, body) {
            cb(error, body / 1);
        });
    }
};