﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool ReadCharacterBuff(MySqlDataReader reader, out CharacterBuff result)
        {
            if (reader.Read())
            {
                result = new CharacterBuff();
                result.type = (BuffType)reader.GetByte(0);
                result.dataId = reader.GetInt32(1);
                result.level = reader.GetInt16(2);
                result.buffRemainsDuration = reader.GetFloat(3);
                return true;
            }
            result = CharacterBuff.Empty;
            return false;
        }

        public async UniTask CreateCharacterBuff(MySqlConnection connection, MySqlTransaction transaction, string characterId, CharacterBuff characterBuff)
        {
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterbuff (id, characterId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @level, @buffRemainsDuration)",
                new MySqlParameter("@id", characterId + "_" + characterBuff.type + "_" + characterBuff.dataId),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@type", (byte)characterBuff.type),
                new MySqlParameter("@dataId", characterBuff.dataId),
                new MySqlParameter("@level", characterBuff.level),
                new MySqlParameter("@buffRemainsDuration", characterBuff.buffRemainsDuration));
        }

        public async UniTask<List<CharacterBuff>> ReadCharacterBuffs(string characterId, List<CharacterBuff> result = null)
        {
            if (result == null)
                result = new List<CharacterBuff>();
            await ExecuteReader((reader) =>
            {
                CharacterBuff tempBuff;
                while (ReadCharacterBuff(reader, out tempBuff))
                {
                    result.Add(tempBuff);
                }
            }, "SELECT type, dataId, level, buffRemainsDuration FROM characterbuff WHERE characterId=@characterId ORDER BY buffRemainsDuration ASC",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterBuffs(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterbuff WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
