# uva10975

## Performance analysis.

The **Trie** data structure was used to solve this quiz for it's structure represent a huge gain in the search performance eliminating unecessary queries and, potentially, reducing the final dataset significantly.
This data structure can cost more to be built than a normal collection, but this is compensated for it's query speed. 
Unlike a full string comparison, where one would need to read all the possible words from the matrix for each starting letter as a whole to be abble to make search for matchs, in this datastructure the program can read each char on-the-fly and query the trie downwards until it gets a match or not. One consequence of this behavior is the  ability to end the query without the need to read the complete array of characters for the analised case, once one character in the array returns no match from the trie the search for that iteration can end and the process can move to the next iteration. This represents a huge gain in processing time and overall performance.

## Test Case Samples

```

3
4
hello
bye
one
two
2
3 3
bye
okk
res
3 3
one
wzq
too
5
madam
egg
hello
e
ll
1
6 8
xhellohx
xoezxecx
xlnllbvx
xlmllasx
xeggfodx
xhmadamx
4
b
by
bye
byte
1
6 10
lllllbyte
llllbytel
lllbytell
llbytelll
lbytellll
bytelllll

```

## Output Samples

```

Test Case #1
Query #1
bye 1
Query #2
one 1
two 1

Test Case #2
Query #1
hello 3
e 32
ll 16
egg 1
madam 2

Test Case #3
Query #1
b 48
by 11
byte 9

```