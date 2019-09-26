StarcounterHost="http://127.0.0.1:8080"

echo Removing previous results
rm ./results/starcounter/*

echo Running Warm Up
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=warmup
echo Warm Up Completed

echo Running Shortest Path Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=shortest &> results/starcounter/shortest.log
echo Shortest Path Completed

echo Running Neighbors Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=neighbors &> results/starcounter/neighbors.log
echo Neighbors Completed

echo Running Neighbors 2 Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=neighbors2 &> results/starcounter/neighbors2.log
echo Neighbors 2 Completed

echo Running Neighbors 2 Data Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=neighbors2data &> results/starcounter/neighbors2data.log
echo Neighbors 2 Data Completed

echo Running Single Read Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=singleRead &> results/starcounter/single-read.log
echo Single Read Benchmark Completed

echo Running Single Write Benchmark
nodejs --max-old-space-size=8192 benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=singleWrite &> results/starcounter/single-write.log
echo Single Write Benchmark Completed

echo Running Aggregation Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=aggregation &> results/starcounter/aggregation.log
echo Aggregation Completed

echo Running Hard Path Benchmark
nodejs benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=hardPath &> results/starcounter/hard-path.log
echo Hard Path Completed

echo Running Single Write Sync Benchmark
nodejs --max-old-space-size=8192 benchmark.js starcounter -a=$StarcounterHost/starcounter-mmb -t=singleWriteSync &> results/starcounter/single-write-sync.log
echo Single Write Sync Benchmark Completed