using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScrollController{
    public interface IScrollEvent
    {
        void Deselect();
        void Select();
    }
}

