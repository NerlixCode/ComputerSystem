using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    class Program
    {
        //Rodion	Onyshchuk	d	c	a
        static void Main(string[] args)
        {
            //Int16 multiplicand, multiplier;
            //Console.WriteLine("Enter 16 bits signed multiplicand:");
            //multiplicand = Int16.Parse(Console.ReadLine());
            //Console.WriteLine("Enter 16 bits signed multiplier:");
            //multiplier = Int16.Parse(Console.ReadLine());
            //Task1(multiplicand, multiplier);

            Int16 dividend, divisor;
            Console.WriteLine("Enter 16 bits signed dividend:");
            dividend = Int16.Parse(Console.ReadLine());
            Console.WriteLine("Enter 16 bits signed divisor:");
            divisor = Int16.Parse(Console.ReadLine());
            Task2(dividend, divisor);

            //float a, b;
            //Console.WriteLine("Enter first float signed value:");
            //a = float.Parse(Console.ReadLine());
            //Console.WriteLine("Enter second float signed value:");
            //b = float.Parse(Console.ReadLine());
            //Task3(a, b);
            Console.ReadKey();
        }

        #region Task1
        static void Task1(Int16 multiplicand, Int16 multiplier)
        {
            Int64 A = (Int64)multiplicand << 17,
                S = (Int64)(-multiplicand) << 17,
                P = (multiplier << 1) & 0b0000_0000_0000_0000_1111_1111_1111_1111_0; //fill first 16 bits by zeros
            string A_bits = IntToBinaryString(A),
                S_bits = IntToBinaryString(S);
            Console.WriteLine("Booth's algorithm:");
            for (int i = 1; i < 17; ++i)
            {
                Console.WriteLine("  Step " + i + ":");
                switch (P & 0b11)
                {
                    case 0b01:
                        Console.WriteLine("  \tAdd A:\t{0}\n\tTo P:\t{1}", A_bits, IntToBinaryString(P));
                        P += A;
                        break;
                    case 0b10:
                        Console.WriteLine("  \tAdd S:\t{0}\n\tTo P:\t{1}", S_bits, IntToBinaryString(P));
                        P += S;
                        break;
                }
                Console.WriteLine("  \tShift right:\t" + IntToBinaryString(P));
                P >>= 1;
                Console.WriteLine("  \t\t" + IntToBinaryString(P));
            }
            P >>= 1; //discard the last bit
            Console.WriteLine("  Answer is:\n\tIn decemal: {0}\n\tIn binary: {1}", P, IntToBinaryString(P, true));
        }

        static string IntToBinaryString(Int64 number, bool is_end_result = false)
        {
            const int mask = 1;
            var binary = string.Empty;
            for (int i = (is_end_result ? 1 : 0); i < 33; ++i)
            {
                binary = (i % 4 == 0 ? " " : "") + (number & mask) + binary;
                number >>= 1;
            }

            return binary;
        }
        #endregion

        #region Task2
        static void Task2(Int16 dividend, Int16 divisor)
        {
            Int64 register = 0 | dividend,
                remainder_register_bits = 0b1_1111_1111_1111_1111_0000_0000_0000_0000,
                quotient_register_bits = 0b1111_1111_1111_1111,
                shifted_divisor = divisor << 16,
                shifted_minus_divisor = -divisor << 16;

            const int remainder_bits_amount = 17,
                quotient_bits_amount = 16,
                register_bits_amount = 33;

            Console.WriteLine("\tRegister:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
            for (int i = 0; i < 16; ++i)
            {
                register <<= 1;
                Console.WriteLine("\tShift left:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));

                if ((register >> 32 & 1) == 0)
                {
                    Console.WriteLine("Substract divisor: {0}", RegisterPartToBinaryString(shifted_minus_divisor, remainder_bits_amount, true));
                    register += shifted_minus_divisor;
                    Console.WriteLine("\tRegister:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
                }
                else
                {
                    Console.WriteLine("      Add divisor: {0}", RegisterPartToBinaryString(shifted_divisor, remainder_bits_amount, true));
                    register += shifted_divisor;
                    Console.WriteLine("\tRegister:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
                }

                if ((register >> 32 & 1) == 0)
                {
                    register |= 1;
                    Console.WriteLine("\tSet last quotient bit to 1:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
                }
                else
                    Console.WriteLine("\tSet last quotient bit to 0:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
            }

            if ((register >> 32 & 1) == 1)
            {
                Console.WriteLine("      Add divisor: {0}", RegisterPartToBinaryString(shifted_divisor, remainder_bits_amount, true));
                register += shifted_divisor;
                Console.WriteLine("\tRegister:\n\t\t   {0}", RegisterPartToBinaryString(register, register_bits_amount));
            }

            Console.WriteLine("\tAnswer is:");
            Console.WriteLine("\t\tRemainder:\t    {0} (in decimal: {1})", 
                RegisterPartToBinaryString(register & remainder_register_bits, remainder_bits_amount, true), 
                (register & remainder_register_bits) >> 16);
            Console.WriteLine("\t\tQuotient:\t      {0} (in decimal: {1})", 
                RegisterPartToBinaryString(register & quotient_register_bits, quotient_bits_amount), 
                register & quotient_register_bits);
        }

        static string RegisterPartToBinaryString(Int64 register, byte bits_amount, bool is_divisor = false)
        {
            string result = string.Empty;

            int last_index = is_divisor ? 15 : -1;
            for (int i = bits_amount - 1 + (is_divisor ? 16 : 0); i > last_index; --i)
                result += (register >> i & 1) + (i % 4 == 0 && i != 0 ? " " : "");

            return result;
        }
        #endregion

        #region Task3
        static void Task3(float a, float b)
        {
            int bias = (int)(Math.Pow(2, 7) - 1);

            bool is_adding = a * b >= 0;
            if (Math.Abs(b) > Math.Abs(a))
                (a, b) = (b, a);

            Console.WriteLine("Adding {0} (a), to {1} (b)", a, b);
            int a_sign_bit = a < 0 ? 1 : 0,
                b_sign_bit = b < 0 ? 1 : 0;
            (a, b) = (Math.Abs(a), Math.Abs(b));
            int a_int_bits = (int)a,
                b_int_bits = (int)b;
            a -= a_int_bits;
            b -= b_int_bits;

            FloatBits(a, out Int32 a_float_bits);
            FloatBits(b, out Int32 b_float_bits);

            Console.WriteLine("  Convert \"a\" to binary (without exponent and normalization):\n\t{0}",
                ResultToBinaryString(a_sign_bit, 0, a_float_bits));
            Console.WriteLine("  Convert \"b\" to binary (without exponent and normalization):\n\t{0}",
                ResultToBinaryString(b_sign_bit, 0, b_float_bits));

            Int16 exp_a = Normalize(a_int_bits, ref a_float_bits),
                exp_b = Normalize(b_int_bits, ref b_float_bits);

            byte exponent_a = (byte)(exp_a + bias),
                exponent_b = (byte)(exp_b + bias);

            string a_float_bits_string = ResultToBinaryString(a_sign_bit, exponent_a, a_float_bits);
            Console.WriteLine("  Normalize \"a\":\n\t{0}", a_float_bits_string);
            Console.WriteLine("  Normalize \"b\" :\n\t{0}", ResultToBinaryString(b_sign_bit, exponent_b, b_float_bits));

            b_float_bits >>= exp_a - exp_b;

            string b_float_bits_string = ResultToBinaryString(b_sign_bit, exponent_b, b_float_bits);
            Console.WriteLine("  Shift left \"b\" on {0}:\n\t{1}", exp_a - exp_b, b_float_bits_string);
            Console.WriteLine("  Adding \"a\" to \"b\":\n\t{0}\n      +\t{1}", a_float_bits_string, b_float_bits_string);

            Int32 result = is_adding ? a_float_bits + b_float_bits : a_float_bits - b_float_bits;
            NormilizeResult(ref result, ref exp_a, is_adding);
            exponent_a = (byte)(exp_a + bias);

            Console.WriteLine("  Answer is:\n\tIn decemal: {0}\n\tIn binary: {1}",
                ConvertToDecimal(exp_a, result, a_sign_bit), ResultToBinaryString(a_sign_bit, exponent_a, result));
        }

        static void FloatBits(float value, out Int32 float_bits)
        {
            const int amount_of_mantisa_bits = 23;
            int i = 0;

            float_bits = 0;
            while (value != 0 && i < 22) // check on overflow
            {
                value *= 2;
                if (value >= 1)
                {
                    float_bits |= 1;
                    value -= 1;
                }
                float_bits <<= 1;
                ++i;
            }
            float_bits <<= amount_of_mantisa_bits - i - 1;
        }

        static Int16 Normalize(int value, ref Int32 float_bits)
        {
            Int16 exp = 0;
            Int32 hidden_one = 1 << 23;

            if (value > 0)
            {
                while (value > 1)
                {
                    ++exp;
                    float_bits >>= 1;
                    float_bits |= (value & 1) << 22;
                    value >>= 1;
                }
                float_bits |= hidden_one;
            }
            else
            {
                if (float_bits == 0)
                    exp = (Int16)(-Math.Pow(2, 7) + 1);
                else
                    do
                    {
                        --exp;
                        float_bits <<= 1;
                    } while ((float_bits & hidden_one) != hidden_one);
            }

            return exp;
        }

        static void NormilizeResult(ref Int32 result, ref Int16 exp, bool is_adding)
        {
            Int32 hidden_one = 1 << 23;

            if ((result & hidden_one) == hidden_one)
                return;

            if (is_adding)
                do
                {
                    ++exp;
                    result >>= 1;
                } while ((result & hidden_one) != hidden_one);
            else
                do
                {
                    --exp;
                    result <<= 1;
                } while ((result & hidden_one) != hidden_one);
        }

        static string ResultToBinaryString(int sign_bit, byte exponent, Int32 result)
        {
            string result_string = sign_bit + " | ";
            for (int i = 7; i >= 0; --i)
                result_string += exponent >> i & 1;
            result_string += " | ";
            for (int i = 22; i >= 0; --i)
                result_string += result >> i & 1;

            return result_string;
        }

        static float ConvertToDecimal(Int16 exp_a, Int32 mantissa, int sign_bit)
        {
            float result = 0,
                multiplier = (float)Math.Pow(2, exp_a);


            for (int i = 23; i >= 0; --i, multiplier /= 2)
                result += multiplier * (mantissa >> i & 1);
            if (sign_bit == 1)
                result = -result;
            return result;
        }
        #endregion
    }
}
