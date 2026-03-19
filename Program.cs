

List<(int row, int column)> grid = [];
for (int i= 1; i<=4; i++)
{
    for (int j = 1; j<=4; j++)
    {
        grid.Add((i, j));
    }
}
foreach ((int row, int column) in grid)
{
    Console.WriteLine($"Row: {row}, Column: {column}");
}


