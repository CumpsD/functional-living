namespace FunctionalLiving.Knx.Parser.Tests

module Category10_Time =

    open Xunit
    open System
    open FunctionalLiving.Knx.Parser

    let data = toMemberData [
      (0xEFuy, 0x28uy, 0x02uy, Some Sunday, TimeSpan(15, 40, 2))
    ]

    [<Theory; MemberData("data")>]
    let ``10.001 time of day test`` telegramBytes expected =
        verifyParser parseTime telegramBytes expected
