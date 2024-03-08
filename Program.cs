using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace testeSerialVSC;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();

        string? op = "1";

//                                     PA15    PA1    PA8    PA9    PA0    PB5    PB4    PB3    PA2    PB12
        byte[] iosModes = new byte[10]{0x01,   0x00,  0x02,  0x01,  0x03,  0x03,  0x02,  0x00,  0x00,  0x01};

//                                     PA15    PA1    PA8    PA9    PA0    PB5    PB4    PB3    PA2    PB12
        byte[] iosZones = new byte[10]{0x03,   0x04,  0x03,  0x01,  0x03,  0x04,  0x01,  0x02,  0x03,  0x00};

//                                      Timer0      Timer1      Timer2      Timer3      Timer4
        byte[] iosTimers = new byte[10]{0x13, 0xD6, 0x4F, 0xA3, 0x12, 0x8E, 0xBC, 0xDF, 0x10, 0x43};

        //O buffer com os modos, as zonas e os timers de zonas dos IOs.
        byte[] bufferIoConfig = new byte[30];
        iosModes.CopyTo(bufferIoConfig, 0);
        iosZones.CopyTo(bufferIoConfig, 10);
        iosTimers.CopyTo(bufferIoConfig, 20);

        byte crcHigh = CRC16.CalcAndGetHigh(bufferIoConfig);
        byte crcLow = CRC16.CalcAndGetLow(bufferIoConfig);

        SerialPort serial = new SerialPort("COM1", 115200);

        try
        {
            serial.Open();

            Console.WriteLine("Conexão com o dispositivo feita com sucesso.\n");

            //Copiando informações do array de zonas dos IOS para o buffer a ser escrito na serial.
            byte[] buffer = new byte[32];
            bufferIoConfig.CopyTo(buffer, 0);

            //Incluindo o CRC nos dois últimos bytes do buffer.
            buffer[30] = crcHigh;
            buffer[31] = crcLow;

            while (true)
            {
                Console.Write("'0' para encerrar. Qualquer coisa para escrever na serial: ");
                op = Console.ReadLine();

                if (op == "0")
                {
                    break;

                } else {
                    Console.WriteLine("Enviado na serial!\n");
                    serial.Write(buffer, 0, buffer.Length);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Não foi possível se conectar ao dispositivo.\n{0}", ex.Message);
        }

        serial.Close();
    }
}
