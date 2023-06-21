namespace Godot.Net.ThirdParty.Icu;

public enum UScriptCode
{
    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    INVALID_CODE = -1,

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    COMMON = 0,  /* Zyyy */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    INHERITED = 1,  /* Zinh */ /* "Code for inherited script", for non-spacing combining marks; also Qaai */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    ARABIC = 2,  /* Arab */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    ARMENIAN = 3,  /* Armn */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    BENGALI = 4,  /* Beng */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    BOPOMOFO = 5,  /* Bopo */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    CHEROKEE = 6,  /* Cher */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    COPTIC = 7,  /* Copt */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    CYRILLIC = 8,  /* Cyrl */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    DESERET = 9,  /* Dsrt */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    DEVANAGARI = 10,  /* Deva */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    ETHIOPIC = 11,  /* Ethi */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    GEORGIAN = 12,  /* Geor */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    GOTHIC = 13,  /* Goth */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    GREEK = 14,  /* Grek */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    GUJARATI = 15,  /* Gujr */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    GURMUKHI = 16,  /* Guru */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    HAN = 17,  /* Hani */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    HANGUL = 18,  /* Hang */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    HEBREW = 19,  /* Hebr */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    HIRAGANA = 20,  /* Hira */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    KANNADA = 21,  /* Knda */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    KATAKANA = 22,  /* Kana */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    KHMER = 23,  /* Khmr */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    LAO = 24,  /* Laoo */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    LATIN = 25,  /* Latn */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    MALAYALAM = 26,  /* Mlym */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    MONGOLIAN = 27,  /* Mong */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    MYANMAR = 28,  /* Mymr */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    OGHAM = 29,  /* Ogam */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    OLD_ITALIC = 30,  /* Ital */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    ORIYA = 31,  /* Orya */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    RUNIC = 32,  /* Runr */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    SINHALA = 33,  /* Sinh */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    SYRIAC = 34,  /* Syrc */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    TAMIL = 35,  /* Taml */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    TELUGU = 36,  /* Telu */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    THAANA = 37,  /* Thaa */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    THAI = 38,  /* Thai */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    TIBETAN = 39,  /* Tibt */
    /** Canadian_Aboriginal script. @stable ICU 2.6 */
    CANADIAN_ABORIGINAL = 40,  /* Cans */
    /** Canadian_Aboriginal script (alias). @stable ICU 2.2 */
    UCAS = CANADIAN_ABORIGINAL,

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    YI = 41,  /* Yiii */
    /* New scripts in Unicode 3.2 */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    TAGALOG = 42,  /* Tglg */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    HANUNOO = 43,  /* Hano */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    BUHID = 44,  /* Buhd */

    /// <summary>
    /// @stable ICU 2.2
    /// </summary>
    TAGBANWA = 45,  /* Tagb */

    /* New scripts in Unicode 4 */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    BRAILLE = 46,  /* Brai */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    CYPRIOT = 47,  /* Cprt */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    LIMBU = 48,  /* Limb */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    LINEAR_B = 49,  /* Linb */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    OSMANYA = 50,  /* Osma */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    SHAVIAN = 51,  /* Shaw */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    TAI_LE = 52,  /* Tale */

    /// <summary>
    /// @stable ICU 2.6
    /// </summary>
    UGARITIC = 53,  /* Ugar */

    /** New script code in Unicode 4.0.1 @stable ICU 3.0 */
    KATAKANA_OR_HIRAGANA = 54,/*Hrkt */

    /* New scripts in Unicode 4.1 */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    BUGINESE = 55, /* Bugi */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    GLAGOLITIC = 56, /* Glag */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    KHAROSHTHI = 57, /* Khar */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    SYLOTI_NAGRI = 58, /* Sylo */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    NEW_TAI_LUE = 59, /* Talu */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    TIFINAGH = 60, /* Tfng */

    /// <summary>
    /// @stable ICU 3.4
    /// </summary>
    OLD_PERSIAN = 61, /* Xpeo */

    /* New script codes from Unicode and ISO 15924 */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    BALINESE = 62, /* Bali */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    BATAK = 63, /* Batk */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    BLISSYMBOLS = 64, /* Blis */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    BRAHMI = 65, /* Brah */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    CHAM = 66, /* Cham */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    CIRTH = 67, /* Cirt */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    OLD_CHURCH_SLAVONIC_CYRILLIC = 68, /* Cyrs */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    DEMOTIC_EGYPTIAN = 69, /* Egyd */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    HIERATIC_EGYPTIAN = 70, /* Egyh */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    EGYPTIAN_HIEROGLYPHS = 71, /* Egyp */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    KHUTSURI = 72, /* Geok */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    SIMPLIFIED_HAN = 73, /* Hans */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    TRADITIONAL_HAN = 74, /* Hant */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    PAHAWH_HMONG = 75, /* Hmng */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    OLD_HUNGARIAN = 76, /* Hung */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    HARAPPAN_INDUS = 77, /* Inds */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    JAVANESE = 78, /* Java */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    KAYAH_LI = 79, /* Kali */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    LATIN_FRAKTUR = 80, /* Latf */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    LATIN_GAELIC = 81, /* Latg */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    LEPCHA = 82, /* Lepc */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    LINEAR_A = 83, /* Lina */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    MANDAIC = 84, /* Mand */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    MANDAEAN = MANDAIC,

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    MAYAN_HIEROGLYPHS = 85, /* Maya */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    MEROITIC_HIEROGLYPHS = 86, /* Mero */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    MEROITIC = MEROITIC_HIEROGLYPHS,

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    NKO = 87, /* Nkoo */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    ORKHON = 88, /* Orkh */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    OLD_PERMIC = 89, /* Perm */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    PHAGS_PA = 90, /* Phag */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    PHOENICIAN = 91, /* Phnx */

    /// <summary>
    /// @stable ICU 52
    /// </summary>
    MIAO = 92, /* Plrd */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    PHONETIC_POLLARD = MIAO,

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    RONGORONGO = 93, /* Roro */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    SARATI = 94, /* Sara */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    ESTRANGELO_SYRIAC = 95, /* Syre */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    WESTERN_SYRIAC = 96, /* Syrj */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    EASTERN_SYRIAC = 97, /* Syrn */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    TENGWAR = 98, /* Teng */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    VAI = 99, /* Vaii */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    VISIBLE_SPEECH = 100, /* Visp */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    CUNEIFORM = 101, /* Xsux */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    UNWRITTEN_LANGUAGES = 102, /* Zxxx */

    /// <summary>
    /// @stable ICU 3.6
    /// </summary>
    UNKNOWN = 103, /* Zzzz */ /* Unknown="Code for uncoded script", for unassigned code points */


    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    CARIAN = 104, /* Cari */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    JAPANESE = 105, /* Jpan */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    LANNA = 106, /* Lana */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    LYCIAN = 107, /* Lyci */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    LYDIAN = 108, /* Lydi */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    OL_CHIKI = 109, /* Olck */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    REJANG = 110, /* Rjng */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    SAURASHTRA = 111, /* Saur */
    /** Sutton SignWriting @stable ICU 3.8 */
    SIGN_WRITING = 112, /* Sgnw */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    SUNDANESE = 113, /* Sund */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    MOON = 114, /* Moon */

    /// <summary>
    /// @stable ICU 3.8
    /// </summary>
    MEITEI_MAYEK = 115, /* Mtei */


    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    IMPERIAL_ARAMAIC = 116, /* Armi */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    AVESTAN = 117, /* Avst */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    CHAKMA = 118, /* Cakm */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    KOREAN = 119, /* Kore */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    KAITHI = 120, /* Kthi */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    MANICHAEAN = 121, /* Mani */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    INSCRIPTIONAL_PAHLAVI = 122, /* Phli */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    PSALTER_PAHLAVI = 123, /* Phlp */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    BOOK_PAHLAVI = 124, /* Phlv */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    INSCRIPTIONAL_PARTHIAN = 125, /* Prti */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    SAMARITAN = 126, /* Samr */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    TAI_VIET = 127, /* Tavt */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    MATHEMATICAL_NOTATION = 128, /* Zmth */

    /// <summary>
    /// @stable ICU 4.0
    /// </summary>
    SYMBOLS = 129, /* Zsym */


    /// <summary>
    /// @stable ICU 4.4
    /// </summary>
    BAMUM = 130, /* Bamu */

    /// <summary>
    /// @stable ICU 4.4
    /// </summary>
    LISU = 131, /* Lisu */

    /// <summary>
    /// @stable ICU 4.4
    /// </summary>
    NAKHI_GEBA = 132, /* Nkgb */

    /// <summary>
    /// @stable ICU 4.4
    /// </summary>
    OLD_SOUTH_ARABIAN = 133, /* Sarb */


    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    BASSA_VAH = 134, /* Bass */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    DUPLOYAN = 135, /* Dupl */

    /** @deprecated ICU 54 Typo, use DUPLOYAN */
    [Obsolete("ICU 54 Typo, use DUPLOYAN")]
    DUPLOYAN_SHORTAND = DUPLOYAN,

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    ELBASAN = 136, /* Elba */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    GRANTHA = 137, /* Gran */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    KPELLE = 138, /* Kpel */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    LOMA = 139, /* Loma */
    /** Mende Kikakui @stable ICU 4.6 */
    MENDE = 140, /* Mend */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    MEROITIC_CURSIVE = 141, /* Merc */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    OLD_NORTH_ARABIAN = 142, /* Narb */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    NABATAEAN = 143, /* Nbat */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    PALMYRENE = 144, /* Palm */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    KHUDAWADI = 145, /* Sind */

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    SINDHI = KHUDAWADI,

    /// <summary>
    /// @stable ICU 4.6
    /// </summary>
    WARANG_CITI = 146, /* Wara */


    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    AFAKA = 147, /* Afak */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    JURCHEN = 148, /* Jurc */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    MRO = 149, /* Mroo */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    NUSHU = 150, /* Nshu */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    SHARADA = 151, /* Shrd */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    SORA_SOMPENG = 152, /* Sora */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    TAKRI = 153, /* Takr */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    TANGUT = 154, /* Tang */

    /// <summary>
    /// @stable ICU 4.8
    /// </summary>
    WOLEAI = 155, /* Wole */


    /// <summary>
    /// @stable ICU 49
    /// </summary>
    ANATOLIAN_HIEROGLYPHS = 156, /* Hluw */

    /// <summary>
    /// @stable ICU 49
    /// </summary>
    KHOJKI = 157, /* Khoj */

    /// <summary>
    /// @stable ICU 49
    /// </summary>
    TIRHUTA = 158, /* Tirh */


    /// <summary>
    /// @stable ICU 52
    /// </summary>
    CAUCASIAN_ALBANIAN = 159, /* Aghb */

    /// <summary>
    /// @stable ICU 52
    /// </summary>
    MAHAJANI = 160, /* Mahj */


    /// <summary>
    /// @stable ICU 54
    /// </summary>
    AHOM = 161, /* Ahom */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    HATRAN = 162, /* Hatr */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    MODI = 163, /* Modi */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    MULTANI = 164, /* Mult */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    PAU_CIN_HAU = 165, /* Pauc */

    /// <summary>
    /// @stable ICU 54
    /// </summary>
    SIDDHAM = 166, /* Sidd */


    /// <summary>
    /// @stable ICU 58
    /// </summary>
    ADLAM = 167, /* Adlm */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    BHAIKSUKI = 168, /* Bhks */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    MARCHEN = 169, /* Marc */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    NEWA = 170, /* Newa */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    OSAGE = 171, /* Osge */


    /// <summary>
    /// @stable ICU 58
    /// </summary>
    HAN_WITH_BOPOMOFO = 172, /* Hanb */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    JAMO = 173, /* Jamo */

    /// <summary>
    /// @stable ICU 58
    /// </summary>
    SYMBOLS_EMOJI = 174, /* Zsye */


    /// <summary>
    /// @stable ICU 60
    /// </summary>
    MASARAM_GONDI = 175, /* Gonm */

    /// <summary>
    /// @stable ICU 60
    /// </summary>
    SOYOMBO = 176, /* Soyo */

    /// <summary>
    /// @stable ICU 60
    /// </summary>
    ZANABAZAR_SQUARE = 177, /* Zanb */


    /// <summary>
    /// @stable ICU 62
    /// </summary>
    DOGRA = 178, /* Dogr */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    GUNJALA_GONDI = 179, /* Gong */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    MAKASAR = 180, /* Maka */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    MEDEFAIDRIN = 181, /* Medf */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    HANIFI_ROHINGYA = 182, /* Rohg */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    SOGDIAN = 183, /* Sogd */

    /// <summary>
    /// @stable ICU 62
    /// </summary>
    OLD_SOGDIAN = 184, /* Sogo */


    /// <summary>
    /// @stable ICU 64
    /// </summary>
    ELYMAIC = 185, /* Elym */

    /// <summary>
    /// @stable ICU 64
    /// </summary>
    NYIAKENG_PUACHUE_HMONG = 186, /* Hmnp */

    /// <summary>
    /// @stable ICU 64
    /// </summary>
    NANDINAGARI = 187, /* Nand */

    /// <summary>
    /// @stable ICU 64
    /// </summary>
    WANCHO = 188, /* Wcho */


    /// <summary>
    /// @stable ICU 66
    /// </summary>
    CHORASMIAN = 189, /* Chrs */

    /// <summary>
    /// @stable ICU 66
    /// </summary>
    DIVES_AKURU = 190, /* Diak */

    /// <summary>
    /// @stable ICU 66
    /// </summary>
    KHITAN_SMALL_SCRIPT = 191, /* Kits */

    /// <summary>
    /// @stable ICU 66
    /// </summary>
    YEZIDI = 192, /* Yezi */


    /// <summary>
    /// @stable ICU 70
    /// </summary>
    CYPRO_MINOAN = 193, /* Cpmn */

    /// <summary>
    /// @stable ICU 70
    /// </summary>
    OLD_UYGHUR = 194, /* Ougr */

    /// <summary>
    /// @stable ICU 70
    /// </summary>
    TANGSA = 195, /* Tnsa */

    /// <summary>
    /// @stable ICU 70
    /// </summary>
    TOTO = 196, /* Toto */

    /// <summary>
    /// @stable ICU 70
    /// </summary>
    VITHKUQI = 197, /* Vith */


    /// <summary>
    /// @stable ICU 72
    /// </summary>
    KAWI = 198, /* Kawi */

    /// <summary>
    /// @stable ICU 72
    /// </summary>
    NAG_MUNDARI = 199, /* Nagm */

    [Obsolete("ICU 58 The numeric value may change over time")]
    CODE_LIMIT = 200
}
