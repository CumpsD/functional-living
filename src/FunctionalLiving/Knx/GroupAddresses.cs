namespace FunctionalLiving.Knx
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class GroupAddresses
    {
        // 1.001 Switches
        public static readonly IDictionary<string, string> Switches = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/1/0", "Verlichting - Leefruimte - Spots TV - Aan/Uit" },
            { "0/1/1", "Verlichting - Leefruimte - Spots Zetel - Aan/Uit" },
            { "0/1/2", "Verlichting - Leefruimte - Spots Piano - Aan/Uit" },
            { "0/1/3", "Verlichting - Keuken - Spots - Aan/Uit" },
            { "0/1/4", "Verlichting - Trap - Spots - Aan/Uit" },
            { "0/1/5", "Verlichting - Nachthal - Spots - Aan/Uit" },
            { "0/1/6", "Verlichting - Badkamer - Spots - Aan/Uit" },
            { "0/1/7", "Verlichting - Bureau - Spots Binnencirkel - Aan/Uit" },
            { "0/1/8", "Verlichting - Bureau - Spots Buitencirkel - Aan/Uit" },
            { "0/1/9", "Verlichting - Toilet Boven - Spot - Aan/Uit" },
            { "0/1/10", "Verlichting - Garage - Centraal - Aan/Uit" },
            { "0/1/11", "Verlichting - Berging - Centraal - Aan/Uit" },
            { "0/1/12", "Verlichting - Inkomhal - Centraal - Aan/Uit" },
            { "0/1/13", "Verlichting - Toilet Beneden - Wastafel - Aan/Uit" },
            { "0/1/14", "Verlichting - Leefruimte - Eettafel - Aan/Uit" },
            { "0/1/15", "Verlichting - Badkamer - Makeup - Aan/Uit" },
            { "0/1/16", "Verlichting - Abby - Centraal - Aan/Uit" },
            { "0/1/17", "Verlichting - Opslag - Centraal - Aan/Uit" },
            { "0/1/18", "Verlichting - David - Centraal - Aan/Uit" },
            { "0/1/19", "Verlichting - Badkamer - Spiegel - Aan/Uit" },
            { "0/1/20", "Verlichting - Garage - Onder Trap - Aan/Uit" },
            { "0/1/21", "Verlichting - Zoldertrap - Wand - Aan/Uit" },
            { "0/1/22", "Verlichting - Bureau - Centraal - Aan/Uit" },
            { "0/1/23", "Verlichting - Keuken - Eiland - Aan/Uit" },
            { "0/1/24", "Verlichting - Leefruimte - Spots Dressoir - Aan/Uit" },

            // The 1 range are addresses which provide feedback
            { "1/1/0", "Toestellen - Dampkap - Aan/Uit" },
            { "1/1/1", "Toestellen - Kookplaat - Aan/Uit" },
            { "1/1/2", "Toestellen - Oven - Aan/Uit" },
            { "1/1/3", "Toestellen - Microgolf - Aan/Uit" },
            { "1/1/4", "Toestellen - Vaatwas - Aan/Uit" },
            { "1/1/5", "Toestellen - Droogkast - Aan/Uit" },
            { "1/1/6", "Toestellen - Wasmachine - Aan/Uit" },
            { "1/1/7", "Toestellen - Diepvries - Aan/Uit" },
            { "1/1/8", "Toestellen - Koelkast - Aan/Uit" },
            { "1/1/9", "Toestellen - Handdoekdroger - Aan/Uit" },
            { "1/1/10", "Toestellen - Stopcontacten Buiten - Aan/Uit" },

            { "1/3/3", "Boiler - Pomp - Aan/Uit" },
            { "1/3/7", "Zonnecollector - Pomp - Aan/Uit" },
            { "1/3/13", "Vloerverwarming - Pomp - Aan/Uit" },
            { "1/3/19", "Warmwater - Pomp - Aan/Uit" },

            // The 2 range are addresses which actually control the state
            { "2/1/1", "Verlichting - Voordeur - Aan/Uit" },
            { "2/1/10", "Verlichting - Tuin - Zijmuur - Aan/Uit" },
            { "2/1/11", "Verlichting - Tuin - Terras - Aan/Uit" },

            { "2/2/1", "Verlichting - Inkomhal - Centraal - Aan/Uit" },
            { "2/2/2", "Verlichting - Toilet Beneden - Wastafel - Aan/Uit" },
            { "2/2/10", "Verlichting - Leefruimte - Eettafel - Aan/Uit" },
            { "2/2/11", "Verlichting - Leefruimte - Spots Piano - Aan/Uit" },
            { "2/2/12", "Verlichting - Leefruimte - Spots TV - Aan/Uit" },
            { "2/2/13", "Verlichting - Leefruimte - Spots Zetel - Aan/Uit" },
            { "2/2/14", "Verlichting - Leefruimte - Spots TV + Zetel - Aan/Uit" },
            { "2/2/15", "Verlichting - Leefruimte - Spots Dressoir - Aan/Uit" },
            { "2/2/20", "Verlichting - Keuken - Eiland - Aan/Uit" },
            { "2/2/21", "Verlichting - Keuken - Spots - Aan/Uit" },
            { "2/2/30", "Verlichting - Berging - Centraal - Aan/Uit" },
            { "2/2/40", "Verlichting - Garage - Centraal - Aan/Uit" },
            { "2/2/41", "Verlichting - Garage - Onder Trap - Aan/Uit" },

            { "2/3/1", "Verlichting - Trap - Spots - Aan/Uit" },
            { "2/3/2", "Verlichting - Trap - Muur - Aan/Uit" },
            { "2/3/3", "Verlichting - Trap - Spots + Muur - Aan/Uit" },
            { "2/3/10", "Verlichting - Nachthal - Spots - Aan/Uit" },
            { "2/3/11", "Verlichting - Trap + Nachthal - Spots - Aan/Uit" },
            { "2/3/20", "Verlichting - Badkamer - Spots - Aan/Uit" },
            { "2/3/21", "Verlichting - Badkamer - Spiegel - Aan/Uit" },
            { "2/3/22", "Verlichting - Badkamer - Makeup - Aan/Uit" },
            { "2/3/30", "Verlichting - Bureau - Centraal - Aan/Uit" },
            { "2/3/31", "Verlichting - Bureau - Spots Buitencirkel - Aan/Uit" },
            { "2/3/32", "Verlichting - Bureau - Spots Binnencirkel - Aan/Uit" },
            { "2/3/33", "Verlichting - Bureau - Spots - Aan/Uit" },
            { "2/3/40", "Verlichting - David - Centraal - Aan/Uit" },
            { "2/3/41", "Verlichting - David - Bed Links - Aan/Uit" },
            { "2/3/42", "Verlichting - David - Bed Rechts - Aan/Uit" },
            { "2/3/50", "Verlichting - Opslag - Centraal - Aan/Uit" },
            { "2/3/60", "Verlichting - Abby - Centraal - Aan/Uit" },
            { "2/3/70", "Verlichting - Toilet Boven - Spot - Aan/Uit" },

            { "2/4/1", "Verlichting - Trap - Wand - Aan/Uit" },

            // The 5 range are addresses which actually control the state
            { "5/1/0", "Toestellen - Stopcontacten Buiten - Aan/Uit" },
            { "5/2/0", "Toestellen - Dampkap - Aan/Uit" },
            { "5/2/1", "Toestellen - Kookplaat - Aan/Uit" },
            { "5/2/2", "Toestellen - Oven - Aan/Uit" },
            { "5/2/3", "Toestellen - Microgolf - Aan/Uit" },
            { "5/2/4", "Toestellen - Vaatwas - Aan/Uit" },
            { "5/2/5", "Toestellen - Droogkast - Aan/Uit" },
            { "5/2/6", "Toestellen - Wasmachine - Aan/Uit" },
            { "5/2/7", "Toestellen - Diepvries - Aan/Uit" },
            { "5/2/8", "Toestellen - Koelkast - Aan/Uit" },
            { "5/3/0", "Toestellen - Handdoekdroger - Aan/Uit" },

        });

        // 1.002 Toggles (boolean)
        public static readonly IDictionary<string, string> Toggles = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/5/0", "Beweging in inkomhal" },
            { "0/5/1", "Beweging in toilet beneden" },
            { "0/5/2", "Beweging in berging" },
            { "0/5/3", "Beweging in garage" },
            { "0/5/4", "Beweging in nachthal trap" },
            { "0/5/5", "Beweging in nachthal badkamer" },
            { "0/5/6", "Beweging in badkamer" },
            { "0/5/7", "Beweging in toilet boven" },

            { "1/0/1", "Neerslag buiten" },
            { "1/0/2", "Droog buiten" },
        });

        // 5.001 Percentages (0..100%)
        public static readonly IDictionary<string, string> Percentages = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/2/0", "Verlichting - Leefruimte - Spots TV - Dimmen" },
            { "0/2/1", "Verlichting - Leefruimte - Spots Zetel - Dimmen" },
            { "0/2/2", "Verlichting - Leefruimte - Spots Piano - Dimmen" },
            { "0/2/3", "Verlichting - Keuken - Spots - Dimmen" },
            { "0/2/4", "Verlichting - Trap - Spots - Dimmen" },
            { "0/2/5", "Verlichting - Nachthal - Spots - Dimmen" },
            { "0/2/6", "Verlichting - Badkamer - Spots - Dimmen" },
            { "0/2/7", "Verlichting - Bureau - Spots Binnencirkel - Dimmen" },
            { "0/2/8", "Verlichting - Bureau - Spots Buitencirkel - Dimmen" },
            { "0/2/9", "Verlichting - Abby - Centraal - Dimmen" },
            { "0/2/10", "Verlichting - Opslag - Centraal - Dimmen" },
            { "0/2/11", "Verlichting - David - Centraal - Dimmen" },
            { "0/2/12", "Verlichting - Bureau - Centraal - Dimmen" },
            { "0/2/13", "Verlichting - Toilet Boven - Spot - Dimmen" },
            { "0/2/14", "Verlichting - Keuken - Eiland - Dimmen" },
            { "0/2/15", "Verlichting - Leefruimte - Spots Dressoir - Dimmen" },

            // The 3 range are addresses which actually control the state
            // TODO: Double check if it is relative or absolute dimming
            { "3/2/11", "Verlichting - Leefruimte - Spots Piano - Dimmen Relatief" },
            { "3/2/12", "Verlichting - Leefruimte - Spots TV - Dimmen Relatief" },
            { "3/2/13", "Verlichting - Leefruimte - Spots Zetel - Dimmen Relatief" },
            { "3/2/14", "Verlichting - Leefruimte - Spots TV + Zetel - Dimmen Relatief" },
            { "3/2/15", "Verlichting - Leefruimte - Spots Dressoir - Dimmen Relatief" },
            { "3/2/20", "Verlichting - Keuken - Eiland - Dimmen Relatief" },
            { "3/2/21", "Verlichting - Keuken - Spots - Dimmen Relatief" },

            { "3/3/1", "Verlichting - Trap - Spots - Dimmen Relatief" },
            { "3/3/10", "Verlichting - Nachthal - Spots - Dimmen Relatief" },
            { "3/3/20", "Verlichting - Badkamer - Spots - Dimmen Relatief" },
            { "3/3/30", "Verlichting - Bureau - Centraal - Dimmen Relatief" },
            { "3/3/31", "Verlichting - Bureau - Spots Buitencirkel - Dimmen Relatief" },
            { "3/3/32", "Verlichting - Bureau - Spots Binnencirkel - Dimmen Relatief" },
            { "3/3/33", "Verlichting - Bureau - Spots - Dimmen Relatief" },
            { "3/3/40", "Verlichting - David - Centraal - Dimmen Relatief" },
            { "3/3/50", "Verlichting - Opslag - Centraal - Dimmen Relatief" },
            { "3/3/60", "Verlichting - Abby - Centraal - Dimmen Relatief" },
            { "3/3/70", "Verlichting - Toilet Boven - Spot - Dimmen Relatief" },

            // The 4 range are addresses which actually control the state
            // TODO: Double check if it is relative or absolute dimming
            { "4/2/11", "Verlichting - Leefruimte - Spots Piano - Dimmen Absoluut" },
            { "4/2/12", "Verlichting - Leefruimte - Spots TV - Dimmen Absoluut" },
            { "4/2/13", "Verlichting - Leefruimte - Spots Zetel - Dimmen Absoluut" },
            { "4/2/14", "Verlichting - Leefruimte - Spots TV + Zetel - Dimmen Absoluut" },
            { "4/2/15", "Verlichting - Leefruimte - Spots Dressoir - Dimmen Absoluut" },
            { "4/2/20", "Verlichting - Keuken - Eiland - Dimmen Absoluut" },
            { "4/2/21", "Verlichting - Keuken - Spots - Dimmen Absoluut" },

            { "4/3/1", "Verlichting - Trap - Spots - Dimmen Absoluut" },
            { "4/3/10", "Verlichting - Nachthal - Spots - Dimmen Absoluut" },
            { "4/3/20", "Verlichting - Badkamer - Spots - Dimmen Absoluut" },
            { "4/3/30", "Verlichting - Bureau - Centraal - Dimmen Absoluut" },
            { "4/3/31", "Verlichting - Bureau - Spots Buitencirkel - Absoluut Relatief" },
            { "4/3/32", "Verlichting - Bureau - Spots Binnencirkel - Absoluut Relatief" },
            { "4/3/33", "Verlichting - Bureau - Spots - Dimmen Absoluut" },
            { "4/3/40", "Verlichting - David - Centraal - Dimmen Absoluut" },
            { "4/3/50", "Verlichting - Opslag - Centraal - Dimmen Absoluut" },
            { "4/3/60", "Verlichting - Abby - Centraal - Dimmen Absoluut" },
            { "4/3/70", "Verlichting - Toilet Boven - Spot - Dimmen Absoluut" },
        });

        // 7.007 Time (h)
        public static readonly IDictionary<string, string> Duration = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/7/2", "Aantal uren activiteit handdoekdroger" },
            { "0/7/4", "Aantal uren activiteit oven" },
            { "0/7/6", "Aantal uren activiteit microgolf" },
            { "0/7/8", "Aantal uren activiteit stopcontacten buiten" },
            { "0/7/10", "Aantal uren activiteit droogkast" },
            { "0/7/12", "Aantal uren activiteit wasmachine" },
            { "0/7/14", "Aantal uren activiteit diepvries" },
            { "0/7/16", "Aantal uren activiteit koelkast" },
            { "0/7/18", "Aantal uren activiteit vaatwas" },
            { "0/7/20", "Aantal uren activiteit dampkap" },

            // The 1 range are addresses which provide feedback
            { "1/3/2", "Boiler - Aantal uur brander" },
            { "1/3/8", "Zonnecollector - Aantal uren gedraaid" },
        });

        // 7.012 Current (mA)
        public static readonly IDictionary<string, string> Current = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/7/3", "Huidig verbruik handdoekdroger" },
            { "0/7/5", "Huidig verbruik oven" },
            { "0/7/7", "Huidig verbruik microgolf" },
            { "0/7/9", "Huidig verbruik stopcontacten buiten" },
            { "0/7/11", "Huidig verbruik droogkast" },
            { "0/7/13", "Huidig verbruik wasmachine" },
            { "0/7/15", "Huidig verbruik diepvries" },
            { "0/7/17", "Huidig verbruik koelkast" },
            { "0/7/19", "Huidig verbruik vaatwas" },
            { "0/7/21", "Huidig verbruik dampkap" },
        });

        // 9.001 Temperate (degrees C)
        public static readonly IDictionary<string, string> Temperatures = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/4/0", "Inkomhal (Plafond)" },
            { "0/4/1", "Toilet beneden (Plafond)" },
            { "0/4/2", "Berging (Plafond)" },
            { "0/4/3", "Garage (Plafond)" },
            { "0/4/4", "Nachthal trap (Plafond)" },
            { "0/4/5", "Nachthal badkamer (Plafond)" },
            { "0/4/6", "Badkamer (Plafond)" },
            { "0/4/7", "Toilet boven (Plafond)" },
            { "0/4/8", "Slaapkamer 1 (David)" },
            { "0/4/9", "Slaapkamer 2 (Opslag)" },
            { "0/4/10", "Slaapkamer 3 (Abby)" },
            { "0/4/11", "Bureau" },
            { "0/4/12", "Badkamer" },
            { "0/4/13", "Weerstation" },
            { "0/4/14", "Keuken" },

            // The 1 range are addresses which provide feedback
            { "1/3/0", "Boiler - Buitentemperatuur" },
            { "1/3/1", "Boiler - Temperatuur" },
            { "1/3/5", "Zonnecollector - Zonnecollector temperatuur" },
            { "1/3/6", "Zonnecollector - Zonnecylinder temperatuur" },
            { "1/3/12", "Vloerverwarming - Temperatuur" },
            { "1/3/17", "Warmwater - Temperatuur" },
            { "1/3/18", "Warmwater - Temperatuur Discharge" },
        });

        // 9.004 Light (lux)
        public static readonly IDictionary<string, string> LightStrength = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            // The 0 range are addresses which provide feedback
            { "0/3/0", "Lichtsterkte in inkomhal" },
            { "0/3/1", "Lichtsterkte in toilet beneden" },
            { "0/3/2", "Lichtsterkte in berging" },
            { "0/3/3", "Lichtsterkte in garage" },
            { "0/3/4", "Lichtsterkte in nachthal trap" },
            { "0/3/5", "Lichtsterkte in nachthal badkamer" },
            { "0/3/6", "Lichtsterkte in badkamer" },
            { "0/3/7", "Lichtsterkte in toilet boven" },
            { "0/3/8", "Lichtsterkte buiten 1" },
            { "0/3/9", "Lichtsterkte buiten schemering" },
            { "0/3/10", "Lichtsterkte buiten 2" },
            { "0/3/11", "Lichtsterkte buiten 3" },
        });

        // 9.005 Speed (m/s)
        public static readonly IDictionary<string, string> Speed = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/0/0", "Windsnelheid buiten" },
        });

        // 10.001 Time of day
        public static readonly IDictionary<string, string> Times = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/0/1", "Centrale Tijd" },
        });

        // 11.001 Date
        public static readonly IDictionary<string, string> Dates = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/0/2", "Centrale Datum" },
        });

        // 13.010 Energy (Wh)
        public static readonly IDictionary<string, string> EnergyWattHour = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/3/10", "Zonnecollector - Warmteopbrengst vandaag" },
        });

        // 13.013 Energy (kWh)
        public static readonly IDictionary<string, string> EnergyKiloWattHour = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/3/9", "Zonnecollector - Warmteopbrengst" },
        });
    }
}
