using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.Cliente;

namespace SmartSalesAPI.Mappers
{
    public class ClienteMapper
    {
        public static Cliente ToEntity(CrearClienteDTO clienteDTO)
        {
            return new Cliente
            {
                Nombre = clienteDTO.Nombre,
                Email = clienteDTO.Email,
                Telefono = clienteDTO.Telefono,
            };
        }
        //Metodo sobrecargado
        public static Cliente ToEntity(ModificarClienteDTO clienteDTO)
        {
            return new Cliente
            {
                IdCliente = clienteDTO.Id,
                Nombre = clienteDTO.Nombre,
                Email = clienteDTO.Email,
                Telefono = clienteDTO.Telefono,
            };
        }

        public static ResponderClienteDTO ToDTO(Cliente cliente)
        {
            return new ResponderClienteDTO
            {
                IdCliente = cliente.IdCliente,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
            };
        }

        public static List<ResponderClienteDTO> ToDTOList(List<Cliente> clientes)
        {
            return clientes.Select(ToDTO).ToList();
        }

    }
}
