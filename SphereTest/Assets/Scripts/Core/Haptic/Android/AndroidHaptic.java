package com.colours;

import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;

public class AndroidHaptic {
    private Vibrator _vibrator;

    public void setContext(Context context){
        _vibrator = context.getSystemService(Vibrator.class);
    }
    
    public void vibrate(int type)
    {
        type = getByType(type);
        if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q)
        {
            _vibrator.vibrate(VibrationEffect.createPredefined(type));
        }
    }

    private int getByType(int type)
    {
        if(type == 0) return VibrationEffect.EFFECT_CLICK;
        if(type == 1) return VibrationEffect.EFFECT_TICK;

        return VibrationEffect.EFFECT_HEAVY_CLICK;
    }
}
