using System.Collections.Generic;

namespace WebApiMessageEncryptAndDecrypt.Service
{
    /// <summary>
    ///     Security Settings
    /// </summary>
    public class SecuritySettings
    {
        /// <summary>
        ///     Gets or sets an encryption key
        /// </summary>
        /// <remarks>
        ///     若使用 AES 演算法，需提供 128/192/256 bits 的加密金鑰，換言之金鑰字串長度需為 16/24/32。
        ///     若使用 TripleDES 演算法，需提供 64 bits 的加密金鑰，換言之金鑰字串長度需為 8。
        /// </remarks>
        public string EncryptionKey { get; set; }

        /// <summary>
        ///     Gets or Sets the IV Key
        /// </summary>
        /// <remarks>
        ///     某些加密模式需指定動態初始向量金鑰，如 CBC，EBC 則否，其中 CBC 需提供等同 block size 的 IV key。
        ///     若使用 AES/CBC，需提供 128 bits 的 IV key。
        ///     若使用 TripleDES/CBC，需提供 64 bits 的 IV key。
        /// </remarks>
        public string IvKey { get; set; }
    }
}