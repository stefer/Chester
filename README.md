# Chester
Chess engine with UCI support

Chester is a playground for me to learn chess engines and algorithms and to try out interesting features in C#.

I will not likely accept any pull requests or bug reports.

## Usage

Compile the project and locate the executable.

Use a an [UCI](http://wbec-ridderkerk.nl/html/UCIProtocol.html) compliant Chess client and add Chester as an engine.

Play.

## References

 [ChessProgramming Wiki](https://www.chessprogramming.org/Main_Page)

## TODOS

 - [X] Dotnet 6 / Csharp 10
 - [v] Castling
   - [ ] Current implementation screws up the board somehow as can be seen when playing with Arena
   - [ ] Current implementation requires looking into played moves, 
     Maybe move to use same state as in Fen type?
 - [ ] Iterative deepening of search depth
 - [ ] En Passant
 - [ ] Promoting
 - [ ] Detect mate 
 - [ ] More tests of evaluation / search (does it actually do its job?)
 - [ ] Better opening play 
   * Why not advance the pawns?
 - [ ] Close down process and stop jobs when UCI quit command comes
 - [ ] Tidy up AlphaBeta search code
 - [ ] More fun UCI?
 - [ ] Optimizations
   * Save Benchmark reports and document progress
 - [ ] [Evaluation Hash table](https://www.chessprogramming.org/Evaluation_Hash_Table) 
