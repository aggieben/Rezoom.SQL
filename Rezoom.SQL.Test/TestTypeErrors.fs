﻿module Rezoom.SQL.Test.TestTypeErrors
open System
open NUnit.Framework
open FsUnit
open Rezoom.SQL.Compiler
open Rezoom.SQL.Mapping

[<Test>]
let ``incompatible types can't be compared for equality`` () =
    expectError (Error.cannotUnify "INT" "STRING")
        """
            select g.*, u.*
            from Users u
            left join UserGroupMaps gm on gm.UserId = u.Id
            left join Groups g on g.Id = 'a'
            where g.Name like '%grp%' escape '%'
        """

[<Test>]
let ``unioned queries must have the same number of columns`` () =
    expectError (Error.expectedKnownColumnCount 2 3)
        """
            select 1 a, 2 b, 3 c
            union all
            select 4, 5
        """

[<Test>]
let ``updates must set actual columns`` () =
    expectError (Error.noSuchColumnToSet "Users" "Nane")
        """
            update Users
            set Id = 1, Nane = ''
            where Id > 5
        """

[<Test>]
let ``updated column types must match`` () =
    expectError (Error.cannotUnify "INT" "STRING")
        """
            update Users
            set Id = 'five'
        """

[<Test>]
let ``inserted column types must match`` () =
    expectError (Error.cannotUnify "INT" "STRING")
        """
            insert into Users(Id, Name) values ('one', 'jim')
        """

[<Test>]
let ``inserted columns must exist`` () =
    expectError (Error.noSuchColumn "Goober")
        """
            insert into Users(Goober, Booger) values ('one', 'jim')
        """

[<Test>]
let ``sum argument must be numeric`` () =
    expectError (Error.cannotUnify "<numeric>" "STRING")
        """
            select sum(Name) as Sum from Users
        """

[<Test>]
let ``can't use list-parameter as a scalar result`` () =
    expectError (Error.cannotUnify "<scalar>" "[INT]")
        """
            select @p as x
            from Users
            where Id in @p
        """