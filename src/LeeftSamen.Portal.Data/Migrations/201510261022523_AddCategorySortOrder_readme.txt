
Actions below should be executed before running migration "201510261022523_AddCategorySortOrder"
Please read all steps before executing it in DB.

- Check MarketplaceItems table and make sure all items refer to one of categories with Alias 0, 1, 2 or 3 (check MarketplaceItemCategories for IDs)
- Make sure MarketplaceItemCategories only contains 4 records. Delete all other records from database. What remains should be categories with the following Aliasses (Alias should be Unique) 
    -- 0 = Buurthulp
    -- 1 = Maaltijden
    -- 2 = Spullen te koop
    -- 3 = Spullen te leen

After finishing the steps below the migration can be executed. The seed will automatically update the text fields for the categories.