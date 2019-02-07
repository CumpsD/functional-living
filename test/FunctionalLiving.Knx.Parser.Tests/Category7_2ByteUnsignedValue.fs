namespace FunctionalLiving.Knx.Parser.Tests

module Category7_2ByteUnsignedValue =

    open Xunit
    open FunctionalLiving.Knx.Parser

    let data = toMemberData [
      (0x4Fuy, 0x67uy, 20327.0)
      (0x4Fuy, 0xD9uy, 20441.0)
    ]

    [<Theory; MemberData("data")>]
    let ``7.* 2-byte unsigned value test`` telegramBytes expected =
        let parseTwoByteUnsigned = parseTwoByteUnsigned 1.0
        verifyParser parseTwoByteUnsigned telegramBytes expected

    [<Theory; MemberData("data")>]
    let ``7.012 current (mA) test`` telegramBytes expected =
        let parseTwoByteUnsigned = parseTwoByteUnsigned 1.0
        verifyParser parseTwoByteUnsigned telegramBytes expected
