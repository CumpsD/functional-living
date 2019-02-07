namespace FunctionalLiving.Knx.Parser.Tests

module Category13_4ByteSignedValue =

    open Xunit
    open FunctionalLiving.Knx.Parser

    let data = toMemberData [
      (0x00uy, 0x00uy, 0x0Euy, 0x56uy, 3670L)
      (0x00uy, 0x00uy, 0x1Buy, 0x07uy, 6919L)
    ]

    [<Theory; MemberData("data")>]
    let ``13.010 active energy (Wh) test`` telegramBytes expected =
        verifyParser parseFourByteSigned telegramBytes expected
