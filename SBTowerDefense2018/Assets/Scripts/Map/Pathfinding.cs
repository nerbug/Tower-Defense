﻿using System;
using System.Collections.Generic;

public static class Pathfinding
{
    private static Path CalculatePath(HexTile fromTile, HexTile toTile)
    {
        HexGrid grid = HexGrid.Instance;
        foreach (HexTile tile in grid)
            if(tile != null)
                tile.Reset();

        if (fromTile == null || toTile == null)
            throw new Exception("fromTile or toTile null in Pathfinding class");

        List<HexTile> open = new List<HexTile>();
        HashSet<HexTile> closed = new HashSet<HexTile>();
        open.Add(fromTile);

        while (open.Count > 0)
        {
            //Find tile with smallest fCost
            //TODO: write some sort of priority queue
            HexTile current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].FCost < current.FCost || open[i].FCost == current.FCost)
                    if(open[i].hCost < current.hCost)
                        current = open[i];
            }

            open.Remove(current);
            closed.Add(current);

            if (current == toTile)
            {
                List<HexTile> path = new List<HexTile>();
                HexTile tile = toTile;
                while (tile != fromTile)
                {
                    path.Add(tile.prev);
                    tile = tile.prev;
                }
                path.Reverse();
                path.Add(toTile);
                return new Path(path);
            }

            foreach (HexTile neighbour in grid.GetNeighbours(current))
            {
                if (closed.Contains(neighbour))
                    continue;
                if (neighbour.type != TileType.Empty && neighbour != toTile)
                {
                    if (neighbour.type == TileType.Tower && lastKnownTower == null)
                        lastKnownTower = neighbour;
                    continue;
                }

                int cost = current.gCost + grid.GetDistance(current, neighbour);
                if (cost < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = cost;
                    neighbour.hCost = grid.GetDistance(neighbour, toTile);
                    neighbour.prev = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }
        return null;
    }

    private static HexTile lastKnownTower;

    public static Path GetPath(HexTile fromTile, HexTile toTile)
    {
        lastKnownTower = null;

        //Try to get path to the base
        Path path = CalculatePath(fromTile, toTile);
        if (path != null)
            return path;

        //Get path to the nearest? / last known tower
        if(lastKnownTower != null && path == null)
            path = CalculatePath(fromTile, lastKnownTower);

        return path; 
    }
}
