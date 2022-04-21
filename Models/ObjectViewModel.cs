using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class ObjectViewModel<T>
    {
        public T Object { get; set; }
        public string Action { get; set; } = "Create";
        public bool ReadOnly { get; set; } = false;
        public string Theme { get; set; } = "primary";
        public bool ShowAction { get; set; } = true;
        public bool ShowReadOnlyFields { get; set; } = true;
        public dynamic[] Options { get; set; } = new dynamic[10];
        public string ActionObjectName => Action + Object.GetType().Name;
    }

    public class ObjectViewModelFactory<T>
    {
        public static ObjectViewModel<T> Edit(T obj)
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = "Edit"
            };

        }
        public static ObjectViewModel<T> Create(T obj)
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                ShowReadOnlyFields = false
            };

        }
        public static ObjectViewModel<T> Delete(T obj)
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = "Delete",
                Theme = "danger",
                ShowReadOnlyFields = true,
                ReadOnly = true
            };

        }
        public static ObjectViewModel<T> Details(T obj)
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = "Details",
                ReadOnly = true,
                Theme = "info",
                ShowAction = false,
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
