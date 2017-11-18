using System;
using System.Security.Cryptography;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// CRC32 implementation using a lookup table.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.HashAlgorithm" />
    class Crc32 : HashAlgorithm
    {
        /// <summary>
        /// The lookup table.
        /// </summary>
        private static readonly uint[] Table = {
            0x8DEF02D2, 0x1BDF05A5, 0xA18E0C3C, 0x37BE0B4B,
            0x942B6FD5, 0x021B68A2, 0xB84A613B, 0x2E7A664C,
            0xBF67D9DC, 0x2957DEAB, 0x9306D732, 0x0536D045,
            0xA6A3B4DB, 0x3093B3AC, 0x8AC2BA35, 0x1CF2BD42,
            0xE9FFB5CF, 0x7FCFB2B8, 0xC59EBB21, 0x53AEBC56,
            0xF03BD8C8, 0x660BDFBF, 0xDC5AD626, 0x4A6AD151,
            0xDB776EC1, 0x4D4769B6, 0xF716602F, 0x61266758,
            0xC2B303C6, 0x548304B1, 0xEED20D28, 0x78E20A5F,
            0x45CF6CE9, 0xD3FF6B9E, 0x69AE6207, 0xFF9E6570,
            0x5C0B01EE, 0xCA3B0699, 0x706A0F00, 0xE65A0877,
            0x7747B7E7, 0xE177B090, 0x5B26B909, 0xCD16BE7E,
            0x6E83DAE0, 0xF8B3DD97, 0x42E2D40E, 0xD4D2D379,
            0x21DFDBF4, 0xB7EFDC83, 0x0DBED51A, 0x9B8ED26D,
            0x381BB6F3, 0xAE2BB184, 0x147AB81D, 0x824ABF6A,
            0x135700FA, 0x8567078D, 0x3F360E14, 0xA9060963,
            0x0A936DFD, 0x9CA36A8A, 0x26F26313, 0xB0C26464,
            0x1DAEDEA4, 0x8B9ED9D3, 0x31CFD04A, 0xA7FFD73D,
            0x046AB3A3, 0x925AB4D4, 0x280BBD4D, 0xBE3BBA3A,
            0x2F2605AA, 0xB91602DD, 0x03470B44, 0x95770C33,
            0x36E268AD, 0xA0D26FDA, 0x1A836643, 0x8CB36134,
            0x79BE69B9, 0xEF8E6ECE, 0x55DF6757, 0xC3EF6020,
            0x607A04BE, 0xF64A03C9, 0x4C1B0A50, 0xDA2B0D27,
            0x4B36B2B7, 0xDD06B5C0, 0x6757BC59, 0xF167BB2E,
            0x52F2DFB0, 0xC4C2D8C7, 0x7E93D15E, 0xE8A3D629,
            0xD58EB09F, 0x43BEB7E8, 0xF9EFBE71, 0x6FDFB906,
            0xCC4ADD98, 0x5A7ADAEF, 0xE02BD376, 0x761BD401,
            0xE7066B91, 0x71366CE6, 0xCB67657F, 0x5D576208,
            0xFEC20696, 0x68F201E1, 0xD2A30878, 0x44930F0F,
            0xB19E0782, 0x27AE00F5, 0x9DFF096C, 0x0BCF0E1B,
            0xA85A6A85, 0x3E6A6DF2, 0x843B646B, 0x120B631C,
            0x8316DC8C, 0x1526DBFB, 0xAF77D262, 0x3947D515,
            0x9AD2B18B, 0x0CE2B6FC, 0xB6B3BF65, 0x2083B812,
            0xAD6CBA3F, 0x3B5CBD48, 0x810DB4D1, 0x173DB3A6,
            0xB4A8D738, 0x2298D04F, 0x98C9D9D6, 0x0EF9DEA1,
            0x9FE46131, 0x09D46646, 0xB3856FDF, 0x25B568A8,
            0x86200C36, 0x10100B41, 0xAA4102D8, 0x3C7105AF,
            0xC97C0D22, 0x5F4C0A55, 0xE51D03CC, 0x732D04BB,
            0xD0B86025, 0x46886752, 0xFCD96ECB, 0x6AE969BC,
            0xFBF4D62C, 0x6DC4D15B, 0xD795D8C2, 0x41A5DFB5,
            0xE230BB2B, 0x7400BC5C, 0xCE51B5C5, 0x5861B2B2,
            0x654CD404, 0xF37CD373, 0x492DDAEA, 0xDF1DDD9D,
            0x7C88B903, 0xEAB8BE74, 0x50E9B7ED, 0xC6D9B09A,
            0x57C40F0A, 0xC1F4087D, 0x7BA501E4, 0xED950693,
            0x4E00620D, 0xD830657A, 0x62616CE3, 0xF4516B94,
            0x015C6319, 0x976C646E, 0x2D3D6DF7, 0xBB0D6A80,
            0x18980E1E, 0x8EA80969, 0x34F900F0, 0xA2C90787,
            0x33D4B817, 0xA5E4BF60, 0x1FB5B6F9, 0x8985B18E,
            0x2A10D510, 0xBC20D267, 0x0671DBFE, 0x9041DC89,
            0x3D2D6649, 0xAB1D613E, 0x114C68A7, 0x877C6FD0,
            0x24E90B4E, 0xB2D90C39, 0x088805A0, 0x9EB802D7,
            0x0FA5BD47, 0x9995BA30, 0x23C4B3A9, 0xB5F4B4DE,
            0x1661D040, 0x8051D737, 0x3A00DEAE, 0xAC30D9D9,
            0x593DD154, 0xCF0DD623, 0x755CDFBA, 0xE36CD8CD,
            0x40F9BC53, 0xD6C9BB24, 0x6C98B2BD, 0xFAA8B5CA,
            0x6BB50A5A, 0xFD850D2D, 0x47D404B4, 0xD1E403C3,
            0x7271675D, 0xE441602A, 0x5E1069B3, 0xC8206EC4,
            0xF50D0872, 0x633D0F05, 0xD96C069C, 0x4F5C01EB,
            0xECC96575, 0x7AF96202, 0xC0A86B9B, 0x56986CEC,
            0xC785D37C, 0x51B5D40B, 0xEBE4DD92, 0x7DD4DAE5,
            0xDE41BE7B, 0x4871B90C, 0xF220B095, 0x6410B7E2,
            0x911DBF6F, 0x072DB818, 0xBD7CB181, 0x2B4CB6F6,
            0x88D9D268, 0x1EE9D51F, 0xA4B8DC86, 0x3288DBF1,
            0xA3956461, 0x35A56316, 0x8FF46A8F, 0x19C46DF8,
            0xBA510966, 0x2C610E11, 0x96300788, 0x000000FF,
        };

        private uint _hash;

        /// <summary>
        /// Routes data written to the object into the hash algorithm for computing the hash.
        /// </summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (cbSize += ibStart; ibStart < cbSize; ibStart++)
                _hash = (_hash << 8) ^ Table[(_hash >> 24) ^ array[ibStart]];
        }

        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>
        /// The computed hash code.
        /// </returns>
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(_hash);
        }

        /// <summary>
        /// Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm"></see> class.
        /// </summary>
        public override void Initialize()
        {
            _hash = 0;
        }
    }
}