namespace FunctionalLiving.Knx.Parser.Tests

[<AutoOpen()>]
module TestHelpers =

    open Swensen.Unquote
    open FSharp.Reflection

    let verify = test

    let verifyParser parser telegramBytes expected =
        verify
            <@
                let datapoint =
                    telegramBytes
                    |> parser

                datapoint = expected
            @>

    let toMemberData tuples =
        Seq.map FSharpValue.GetTupleFields tuples
