using CustomerTrackingSystem.Contract;
using CustomerTrackingSystem.DTO;
using CustomerTrackingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using static CustomerTrackingSystem.Helper.Utility;

namespace CustomerTrackingSystem.Controllers
{
    public class CustomerManagementController : Controller
    {
        private readonly IUnitOfWork<Customer> _context;
        private readonly IUnitOfWork<Address> _addressContext;
        private readonly IUnitOfWork<Contact> _contactContext;

        /// <summary>
        /// Customer Management Controller constructor.
        /// </summary>
        /// <param name="context">The unit of work for Customer entities.</param>
        /// <param name="addressContext">The unit of work for Address entities.</param>
        /// <param name="contactContext">The unit of work for Contact entities.</param>
        public CustomerManagementController(IUnitOfWork<Customer> context,
                                            IUnitOfWork<Address> addressContext,
                                            IUnitOfWork<Contact> contactContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _addressContext = addressContext ?? throw new ArgumentNullException(nameof(addressContext));

            _contactContext = contactContext ?? throw new ArgumentNullException(nameof(contactContext));
        }

        /// <summary>
        /// HTTP GET action for viewing customers.
        /// </summary>
        /// <returns>Returns a view containing a list of customers.</returns>

        [HttpGet]
        public async Task<IActionResult> ViewCustomers()
        {
            try
            {
                var customers = await _context.OnLoadItemsAsync();

                var customerList = customers.Select(cust => new CustomerDTO
                {
                    CustomerId = cust.CustomerId,

                    CustomerName = cust.CustomerName,

                    PostalCode = cust.Address?.PostalCode,

                    StreetComplex = cust.Address?.StreetComplex,

                    City = cust.Address?.City,

                    Surburb = cust.Address?.Surburb,

                    ContactPersonEmail = cust.ContactPerson?.ContactPersonEmail,

                    Telephone = cust.ContactPerson?.Telephone,

                    ContactPersonName = cust.ContactPerson?.ContactPersonName,

                    VATNumber = cust.VATNumber

                }).ToList();

                return View(customerList);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// HTTP GET action for adding or editing a customer.
        /// </summary>
        /// <returns>Returns a view to add or edit a customer.</returns>

        [HttpGet]
        public async Task<IActionResult> UpsertCustomer()
        {
            if (Guid.TryParse(HttpContext.Request.Query["CustomerId"], out Guid customerId))
            {
                Customer customer = await _context.OnLoadItemAsync(customerId);

                CustomerDTO dto = null;

                if(customer != null)
                {
                    dto = new()
                    {
                        CustomerId = customerId,

                        City = customer.Address.City,

                        StreetComplex= customer.Address.StreetComplex, 
                        
                        PostalCode= customer.Address.PostalCode,   
                        
                        Surburb = customer.Address.Surburb,

                        ContactPersonEmail = customer.ContactPerson?.ContactPersonEmail,

                        ContactPersonName= customer.ContactPerson?.ContactPersonName,

                        Telephone = customer.ContactPerson?.Telephone, 

                        CustomerName = customer.CustomerName,

                        VATNumber = customer.VATNumber

                    };

                    ViewData["Id"] = customerId;
                }

                return View(dto);
            }

            return View();

        }

        /// <summary>
        /// HTTP POST action for adding or editing a customer.
        /// </summary>
        /// <param name="model">The customer data to be added or edited.</param>
        /// <returns>Returns a view with the updated customer data or an error message.</returns>

        [HttpPost]
        public async Task<IActionResult> UpsertCustomer(CustomerDTO model)
        {
            bool success;

            string message;

            try
            {
                if (model.CustomerId == Guid.Empty)
                {
                    if (_context.DoesEntityExist<Customer>(m => m.ContactPerson.ContactPersonEmail == model.ContactPersonEmail))
                    {
                        TempData["error"] = "Error: Customer already exists";

                        return View();
                    }

                    Customer newCustomer = CreateOrUpdateCustomer(model, "Add");

                    var custObj = await _context.OnItemCreationAsync(newCustomer);

                    if (custObj != null)
                    {
                        int rc = await _context.ItemSaveAsync();

                        if (rc > 0)
                        {
                            success = true;

                            message = "Customer successfully saved";
                        }
                        else
                        {
                            success = false;

                            message = "Error: Unable to save customer!";
                        }
                    }
                    else
                    {
                        success = false;

                        message = "Error: An error occurred while creating customer!";
                    }
                }
                else
                {
                    Customer existingCustomer = await _context.OnLoadItemAsync(model.CustomerId);

                    Customer updatedCustomer = CreateOrUpdateCustomer(model, "Edit", existingCustomer.CustomerId);

                    var custObj = await _context.OnModifyItemAsync(updatedCustomer);

                    if (custObj != null)
                    {
                        success = true;

                        message = "Customer successfully modified";
                    }
                    else
                    {
                        success = false;

                        message = "Error: Unable to modify customer!";
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;

                message = $"Error: An error occurred! {ex.Message}";
            }

            if (success)

                TempData["success"] = message;
           

            else

                TempData["error"] = message;

            return View(model);
        }

        /// <summary>
        /// HTTP GET action for removing a customer.
        /// </summary>
        /// <param name="Id">The ID of the customer to be removed.</param>
        /// <returns>Returns a view indicating whether the customer was successfully removed or not.</returns>

        [Route("/CustomerManagement/RemoveCustomer/{Id}")]
        public async Task<IActionResult> RemoveCustomer(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return NotFound();
            }

            Customer customer = await _context.OnLoadItemAsync(Id);

            if (customer is null)
            {
                return NotFound();
            }

            int del = await _context.OnRemoveItemAsync(Id);

            if (del > 0)
            {
                TempData["success"] = $"Customer successfully deleted";

                return CreatedAtAction("RemoveCustomer", new { Id = customer.CustomerId });
            }

            return View();

        }

        /// <summary>
        /// Creates or updates a Customer entity based on the provided CustomerDTO and operation type.
        /// </summary>
        /// <param name="customerDTO">The CustomerDTO containing the customer data.</param>
        /// <param name="operationType">The type of operation to be performed (Add or Edit).</param>
        /// <param name="existingCustomerId">The existing customer ID (required for Edit operation).</param>
        /// <returns>Returns the newly created or updated Customer entity.</returns>
        private Customer CreateOrUpdateCustomer(CustomerDTO customerDTO, string operationType, Guid? existingCustomerId = null)
        {
            Guid key;

            if (operationType.Equals("Edit", StringComparison.OrdinalIgnoreCase))
            {
                if (existingCustomerId == null)
                {
                    throw new ArgumentException("For 'Edit' operation, an existing CustomerId must be provided.");
                }

                key = existingCustomerId.Value;
            }
            else if (operationType.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                key = Helper.Utility.GenerateGuid();
            }
            else
            {
                throw new ArgumentException("Invalid operation type. Supported values are 'Add' or 'Edit'.");
            }

            Customer customer = new()
            {
                CustomerId = key,

                CustomerName = customerDTO.CustomerName,

                VATNumber = customerDTO.VATNumber,

                Address = new()
                {
                    City = customerDTO.City,

                    PostalCode = customerDTO.PostalCode,

                    StreetComplex = customerDTO.StreetComplex,

                    Surburb = customerDTO.Surburb,

                    CustomerId = key,
                },
                ContactPerson = new()
                {
                    CustomerId = key,

                    ContactPersonEmail = customerDTO.ContactPersonEmail,

                    ContactPersonName = customerDTO.ContactPersonName,

                    Telephone = customerDTO.Telephone,
                }
            };

            return customer;
        }


    }
}
