Raymond Louey
s3853718 
Git Repository: https://github.com/rmit-wdt-sp2-2021/s3853718-a2

Use of C# record type
Found in: Transaction Model
The record type is used to signal that an object is read-only. I've used in it the in the transaction model as I don't expect
to change these at all as each instance of 'transaction' is a historical account of something that has happened. In terms of the
program, knowing that records can not be changed allows for performance improvements when using them. Functions such as the statements
can involve a large amount of transactions, which is where improvements are most visible.