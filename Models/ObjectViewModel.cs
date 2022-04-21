using Project.Migrations;

namespace Project.Models
{
    public class ObjectViewModel<T>
    {
        public T Object { get; set; }
        public string Action { get; set; } = "Create";
        public bool ReadOnly { get; set; } = false;
        public string Theme { get; set; } = "primary";
        public bool ShowAction { get; set; } = true;
    }

    public class ObjectViewModelFactory<T>
    {
        public static ObjectViewModel<T> Edit<T>(T obj)
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = "Edit"
            };
        }
    }

    /*
     
      <div class="form-group">
        <label asp-for="Product.CategoryId">Category</label>
        <div>
            <span asp-validation-for="Product.CategoryId" class="text-danger"></span>
        </div>
        <select asp-for="Product.CategoryId" class="form-control"
                disabled="@Model.ReadOnly"
                asp-items="@(new SelectList(Model.Categories,
"CategoryId", "Name"))">
            <option value="" disabled selected>Choose a Category</option>
        </select>
    </div>
    <div class="form-group">
        <label asp-for="Product.SupplierId">Supplier</label>
        <div>
            <span asp-validation-for="Product.SupplierId" class="text-danger"></span>
        </div>
        <select asp-for="Product.SupplierId" class="form-control"
                disabled="@Model.ReadOnly"
                asp-items="@(new SelectList(Model.Suppliers,
"SupplierId", "Name"))">
            <option value="" disabled selected>Choose a Supplier</option>
        </select>
    </div>
     */
}
