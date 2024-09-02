### Questions:

### Baseline

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks


### Perfomance estimate 

env: 4 GB RAM, 480 GB SSD


### Ideas

1. Set buffer size 64 Kb
2. Count size by reader position
3. Parallel in-memory sorting for chunks
4. Set init primary query
5. Settings and
6. Factory

### TODO 

1. Add duplicates_rate
2. Create make file 
3. Make configs for perfomance testing
4. Add docker env
