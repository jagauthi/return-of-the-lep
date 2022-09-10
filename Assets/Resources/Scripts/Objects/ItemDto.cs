using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemDto {

    protected int itemNumber;
    protected string type;

    public ItemDto(int itemNumber, string type) {
        this.itemNumber = itemNumber;
        this.type = type;
    }

    public string getType() {
        return type;
    }
}
