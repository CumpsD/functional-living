namespace FunctionalLiving.Knx.Parser.Tests

module Category11_Date =

    open Xunit
    open System
    open FunctionalLiving.Knx.Parser

    let data = toMemberData [
        (0x17uy, 0x08uy, 0x0Fuy, new DateTime(2015, 8, 23))
    ]

    [<Theory; MemberData("data")>]
    let ``11.001 date test`` telegramBytes expected =
        verifyParser parseDate telegramBytes expected
