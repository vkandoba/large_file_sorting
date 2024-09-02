### Questions:

### Baseline

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks


### Perfomance estimate 

env: 4 GB RAM, 480 GB SSD


### Ideas

1. Fixed-size prefix in memory for all characters
2. Comparing numbers by string length and maybe as hex representation


### TODO 

1. Docker with perfomance test env
3. Perfomance test files 