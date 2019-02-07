namespace FunctionalLiving.Knx.Parser.Tests

module Category1_SingleBit =

    open Xunit
    open FunctionalLiving.Knx.Parser

    let data = toMemberData [
        (0x00uy, Some SingleBitState.Off)
        (0x01uy, Some SingleBitState.On)
    ]

    [<Theory; MemberData("data")>]
    let ``1.* switching`` telegramBytes expected =
        verifyParser parseSingleBit telegramBytes expected
