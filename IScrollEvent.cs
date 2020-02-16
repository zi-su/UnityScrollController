using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScrollController{
    public interface IScrollEvent
    {
        void OnScrollOut();
        void OnScrollIn();
    }
}

