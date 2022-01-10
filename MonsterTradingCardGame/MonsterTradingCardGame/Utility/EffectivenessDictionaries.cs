using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class EffectivenessDictionaries
    {
        public Dictionary<string, string> spellEffectiveness = new Dictionary<string, string>();
        public Dictionary<string, string> specialTypeEffectiveness = new Dictionary<string, string>();
        Database database = new Database();

        public EffectivenessDictionaries()
        {
            loadSpellEffectiveness();
            loadSpecialTypeEffectiveness();
        }

        private void loadSpellEffectiveness()
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT card_type, effective_against FROM \"card_type_effectiveness\"", conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                spellEffectiveness.Add((string)dr[0], (string)dr[1]);
            }

            conn = database.closeConnection();
        }

        private void loadSpecialTypeEffectiveness()
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT special_type, effective_against FROM \"special_effectiveness\"", conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                specialTypeEffectiveness.Add((string)dr[0], (string)dr[1]);
            }

            conn = database.closeConnection();
        }

        public void printDictionaries()
        {
            foreach (KeyValuePair<string, string> kvp in spellEffectiveness)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            Console.WriteLine("---------------------------------------------");

            foreach (KeyValuePair<string, string> kvp in specialTypeEffectiveness)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
