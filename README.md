Raymond Louey
s3853718 
Git Repository: https://github.com/rmit-wdt-sp2-2021/s3853718-a2

Attempted: Part 4 k) - ASP.NET Core Identity
	   Part 4 l) - Generating Charts - Found in the Customer web page -> log in as 12345678 -> 'My Statements' in the nav bar -> 4101 'History'
		     Chart 1 through 'Transaction type', Chart 2 through 'Balance Trend'

Use of C# record type
Found in: Transaction Model
The record type is used to signal that an object is read-only. I've used in it the in the transaction model as I don't expect
to change these at all as each instance of 'transaction' is a historical account of something that has happened. In terms of the
program, knowing that records can not be changed allows for performance improvements when using them. Functions such as the statements
can involve a large amount of transactions, which is where improvements are most visible.