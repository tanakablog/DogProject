// 00_common/Clipboard.csからios用
extern "C" {
    void _setClip( char *value );
}
void _setClip( char *value ) {
    NSString *strBuf = [NSString stringWithCString: value encoding:NSUTF8StringEncoding];
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    [pasteboard setValue:strBuf forPasteboardType:@"public.text"];

}