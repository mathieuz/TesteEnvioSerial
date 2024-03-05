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
        byte[] iosModes = new byte[10]{0x03,   0x00,  0x03,  0x03,  0x02,  0x01,  0x00,  0x00,  0x01,  0x03};

//                                     PA15    PA1    PA8    PA9    PA0    PB5    PB4    PB3    PA2    PB12
        byte[] iosZones = new byte[10]{0x02,   0x04,  0x01,  0x00,  0x02,  0x03,  0x01,  0x04,  0x02,  0x01};

        //O buffer com os modos e as zonas dos IOs.
        byte[] bufferIoConfig = new byte[20];
        iosModes.CopyTo(bufferIoConfig, 0);
        iosZones.CopyTo(bufferIoConfig, 10);

        byte crcHigh = CRC16.CalcAndGetHigh(bufferIoConfig);
        byte crcLow = CRC16.CalcAndGetLow(bufferIoConfig);

        SerialPort serial = new SerialPort("COM1", 115200);

        try
        {
            serial.Open();

            Console.WriteLine("Conexão com o dispositivo feita com sucesso.\n");

            //Copiando informações do array de zonas dos IOS para o buffer a ser escrito na serial.
            byte[] buffer = new byte[22];

            //Incluindo o CRC nos dois últimos bytes do buffer.
            bufferIoConfig.CopyTo(buffer, 0);
            buffer[20] = crcHigh;
            buffer[21] = crcLow;

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
