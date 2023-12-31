namespace BestPractices.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
  private readonly CustomersDb _context;
  private readonly IMapper _mapper;

  public CustomersController(CustomersDb context, IMapper mapper)
  {
    this._context = context;
    this._mapper = mapper;
  }

  // GET: api/Customers
  [HttpGet]
  public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
  => await this._context.Customers
                        .Select(c => _mapper.Map<CustomerDTO>(c))
                        .ToListAsync();

  // GET: api/Customers/5
  [HttpGet("{id}")]
  public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
  {
    Customer? customer = await this._context.Customers.FindAsync(id);

    if (customer == null)
    {
      return NotFound();
    }
    return _mapper.Map<CustomerDTO>(customer);
  }

  // PUT: api/Customers/5
  // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
  [HttpPut("{id}")]
  public async Task<IActionResult> PutCustomer(int id, CustomerDTO customer)
  {
    if (id != customer.Id)
    {
      return BadRequest();
    }

    Customer cust = _mapper.Map<Customer>(customer);
    this._context.Entry(cust).State = EntityState.Modified;
    this._context.Entry(cust.Address).State = EntityState.Modified;

    try
    {
      await this._context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!CustomerExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  // POST: api/Customers
  // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
  [HttpPost]
  public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customer)
  {
    Customer cust = _mapper.Map<Customer>(customer);

    this._context.Customers.Add(cust);
    await this._context.SaveChangesAsync();

    customer = _mapper.Map<CustomerDTO>(cust);

    return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
  }

  // DELETE: api/Customers/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteCustomer(int id)
  {
    Customer? customer = await this._context.Customers.FindAsync(id);
    if (customer == null)
    {
      return NotFound();
    }

    this._context.Customers.Remove(customer);
    await this._context.SaveChangesAsync();

    return NoContent();
  }

  private bool CustomerExists(int id) => this._context.Customers.Any(e => e.Id == id);
}
