namespace FunctionalLiving.Knx.Parser.Tests

module Category5_Scaling =

    open Xunit
    open FunctionalLiving.Knx.Parser

    let data1 = toMemberData [
        (0xFFuy, 255.0)
        (0x00uy, 0.0)
    ]

    let data2 = toMemberData [
        (0xFFuy, 100.0)
        (0x00uy, 0.0)
        (0x31uy, 19.22)
    ]

    [<Theory; MemberData("data1")>]
    let ``5.* 8-bit unsigned value`` telegramBytes expected =
        let parseScaling = parseScaling 0.0 255.0
        verifyParser parseScaling telegramBytes expected

    [<Theory; MemberData("data2")>]
    let ``5.001 percentage (0..100%)`` telegramBytes expected =
        let parseScaling = parseScaling 0.0 100.0
        verifyParser parseScaling telegramBytes expected
