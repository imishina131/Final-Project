using MatrixUtils.Attributes;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class HeldObjectHandler: MonoBehaviour
{
    public IHeldItem HeldItem { get; private set; }
    //TODO: Swap the old system over to this so I can easily get held object offset data
    public bool TryHoldItem(IHeldItem item)
    {
        if(HasSomethingInHand) return false;
        item.GetTransform().SetParent(transform);
        item.GetTransform().localPosition = item.GetPositionOffset();
        item.GetTransform().localRotation = item.GetRotationOffset();
        HeldItem = item;
        return true;
    }
    
    public bool TryDropItem()
    {
        if(!HasSomethingInHand) return false;
        HeldItem.GetTransform().SetParent(null);
        HeldItem = null;
        return true;
    }
    
    public bool HasSomethingInHand => transform.childCount > 0;
    //[FormerlySerializedAs("objectHoldConstraint"), RequiredField] public TwoBoneIKConstraint m_objectHoldConstraint;
    //TODO: This shouldn't be in update, but since there isn't any easy way to track when an object is picked up at this point. This is part of the reason why moving to the updated system I outlined above would be an improvement on top of allowing asynchronous transitions between holding animations and allowing for offsets
    void Update()
    {
        //m_objectHoldConstraint.weight = !HasSomethingInHand ? 0 : 1;
    }
    public void DestroyObjectsInHand()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    

        /*/| _ ╱|、
         ( •̀ㅅ •́ )
        ＿ノヽノ＼＿
        /　`/ ⌒Ｙ⌒ Ｙ\
      ( 　(三ヽ人　 /　|
        |　ﾉ⌒＼ ￣￣ヽノ
      ヽ＿＿＿＞､＿＿／
        ｜( 王 ﾉ〈
        /ﾐ`ー―彡\
     | ╰    ╯  |
     |     /\   |
     |    /  \  |
     |  /    \ |                  */
}
