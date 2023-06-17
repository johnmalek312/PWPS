using System;

// Token: 0x0200012D RID: 301
public interface IPortal
{
    // Token: 0x06000B98 RID: 2968
    void SetTargetWorldID(string newValue);

    // Token: 0x06000B99 RID: 2969
    string GetTargetWorldID();

    // Token: 0x06000B9A RID: 2970
    void SetTargetEntryPointID(string newValue);

    // Token: 0x06000B9B RID: 2971
    string GetTargetEntryPointID();

    // Token: 0x06000B9C RID: 2972
    void SetName(string newValue);

    // Token: 0x06000B9D RID: 2973
    string GetName();

    // Token: 0x06000B9E RID: 2974
    void SetEntryPointID(string newValue);

    // Token: 0x06000B9F RID: 2975
    string GetEntryPointID();

    // Token: 0x06000BA0 RID: 2976
    void SetIsLocked(bool newValue);

    // Token: 0x06000BA1 RID: 2977
    bool GetIsLocked();
}
