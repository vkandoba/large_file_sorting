### Questions:

### Baseline

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks


### Perfomance estimate 

env: 4 GB RAM, 480 GB SSD

### Ideas

6. Factory
5. Settings and
4. Set init primary query
2. Count size by reader position
3. Parallel in-memory sorting for chunks

### TODO 

1. Add duplicates_rate
2. Create make file 
3. Make configs for perfomance testing
4. Add size of \n\r to gen and dump. Add size duplicates to gen
4. Add docker en