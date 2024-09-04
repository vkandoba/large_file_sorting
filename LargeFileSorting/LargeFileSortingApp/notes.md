### Questions:

### Baseline

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks


### Perfomance estimate 

env: 4 GB RAM, 480 GB SSD

### Ideas

- Set init primary query -- rejected
- Compress duplicates

### TODO 

1. Add usage help string
2. Add docker env