### Questions:

1. What is encoding? UTF-8 is ok?
2. Are there limits for number and string in each line?
3. Are there some reference for corner cases? for examples, file with a few too long lines or a lot of identically line?


### Baseline

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks


### Perfomance estimate 

env: linux, 8 GB RAM, 480 GB SSD


### Ideas

1. Fixed-size prefix in memory for all characters
2. Comparing numbers by string length and maybe as hex representation


### TODO 

1. Function tests
2. Docker with perfomance test env
3. Add random state for generate files 
4. Perfomance test files 