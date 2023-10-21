#import <AudioToolbox/AudioServices.h>

extern "C"
{
    //Not Safe Use Only With @available(iOS 10, *)
    void TriggerHapticStyleFeedback(int style)
    {
        if (style >= 0 && style <= 2)
        {
            UIImpactFeedbackGenerator *generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:(UIImpactFeedbackStyle)style];
            [generator prepare];
            [generator impactOccurred];
        }
        else if (style >= 3 && style <= 5)
        {
            UINotificationFeedbackGenerator *generator = [[UINotificationFeedbackGenerator alloc] init];
            UINotificationFeedbackType feedbackType = (UINotificationFeedbackType)(style - 3);
            [generator prepare];
            [generator notificationOccurred:feedbackType];
        }
    }

    void TriggerHapticStyleFeedbackOld(int style)
    {
       //Light, Success, Medium are bigger than I want
       
       if (style == 2) { // Heavy
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.2 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
                AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
                dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.3 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
                    AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
                });
            });
        } else if (style == 4) { // Warning
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.2 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
                AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
            });
        } else if (style == 5) { // Error
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.3 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
                AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
            });
        }
    }

    void TriggerHapticFeedback(int style)
    {
        if (@available(iOS 10, *))
        {
            TriggerHapticStyleFeedback(style);
        }
        else
        {
            TriggerHapticStyleFeedbackOld(style);
        }
    }
}