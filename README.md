# Starcounter.MultiModelBenchmark

This is an implementation of the open source ArangoDB's benchmark using Starcounter 3.0.

- [NoSQL Performance Benchmark 2018 – MongoDB, PostgreSQL, OrientDB, Neo4j and ArangoDB](https://www.arangodb.com/2018/02/nosql-performance-benchmark-2018-mongodb-postgresql-orientdb-neo4j-arangodb/).

## Setup

`Starcounter.MultiModelBenchmark` requires Starcounter 3.0, which is in preview stage as of 2019.09.26.

- Clone the `Starcounter.MultiModelBenchmark` repository.
- Create `artifacts` folder on the same level as `Starcounter.MultiModelBenchmark` folder.
- Download the latest available Starcounter 3.0 from [starcounter.io](https://starcounter.io/download/) and unzip it into the `artifacts` folder.
- Build `Starcounter.MultiModelBenchmark` with `dotnet` CLI or with Visual Studio 2019.
- Download and unzip test data into a folder.
- Start `Starcounter.MultiModelBenchmark` project with `dotnet` CLI or with Visual Studio 2019.
- Create required database indexes using `create-indexes` end point.
- Import test data using the `import-profiles` and `import-relations` end points. 

See [`Starcounter.MultiModelBenchmark.postman_collection.json`](Starcounter.MultiModelBenchmark.postman_collection.json) [PostMan](https://www.getpostman.com) collection for the detailed REST API description.

### Test data

- [Pokec social network](https://snap.stanford.edu/data/soc-Pokec.html).
- [Profiles](https://snap.stanford.edu/data/soc-pokec-profiles.txt.gz) - 1632803 vertices.
- [Relations](https://snap.stanford.edu/data/soc-pokec-relationships.txt.gz) - 30622564 edges.

## Benchmark

`Starcounter.MultiModelBenchmark` offers two benchmarking options.

### 1. Using Starcounter native C# language binding

In this mode, client is a Starcounter native application with direct database access, which means that database (server) and application (client) reside on the same physical or virtual server.
Starcounter native applications have significant performance advantage over the traditional client-server network communication.

The benchmark payload is located in the [`data/data.zip`](data/data.zip) archive, and has to be manually unzipped into the same folder prior to running any tests.

Use `benchmark` entry point to start required test. See the PostMan collection above.

### 2. Using HTTP REST API

In this mode, client is a classical REST API consumer.

Use [ArangoDB NodeJs benchmarking suite](https://github.com/weinberger/nosql-tests/) to run the tests and collect results.

Starcounter REST API NodeJs implementation is available in the [`scripts/description.js`](scripts/description.js) file.
There is also an `sh` script to perform all the benchmarks using ArangoDB's [NodeJs benchmarker](https://github.com/weinberger/nosql-tests/blob/master/benchmark.js) - [scripts/](benchmark-starcounter.sh).

*Note: NodeJs consumes a lot of RAM when performing multiple parallel HTTP requirests, it might be required to manually increase available amount of RAM for the NodeJs process.*