#include "mbed.h"
#include "LSM6DS3.h"
#include "FunctionPointer.h"
#define AVE_COUNT 100
#include "math.h"

//I2C i2c(I2C_SDA,I2C_SCL);

LSM6DS3 lsm0(I2C_SDA,I2C_SCL,0xD5);//0x6A
LSM6DS3 lsm1(I2C_SDA,I2C_SCL,0xD6);
LSM6DS3 lsm00(PB_4,PA_8,0xD5);
Serial pc(PA_2, PA_3,115200);
//Serial pc(PA_9, PA_10,115200);
//Serial pc(PC_6, PC_7,115200);
DigitalOut led2(PA_5);
DigitalOut Clutch(PA_6);
int status;
uint16_t out_buff[12],test,test1;
uint8_t buff_in[4];
uint8_t flag=0;
float a0,a1,a00;

void rx_event_handeler(int event)
{
    pc.printf("ok:%f:%f:%f:%f:%f:%f:%f:%f:%f:%d\r\n",lsm0.gy,lsm0.ax,lsm0.az,lsm1.gy,lsm1.ax,lsm1.az,lsm00.gy,lsm00.ax,lsm00.az,flag);
    if(buff_in[3]==49){ //49 ASCII value
        Clutch.write(1);
    }
    else if(buff_in[3]==48){
        Clutch.write(0);
    }
}
const event_callback_t rx_event_pointer(rx_event_handeler);

void serial_setup(void){
    //pc.set_flow_control(Serial::RTSCTS,PA_1,PA_0);
    //pc.set_flow_control(Serial::RTSCTS,PA_12,PA_11);
    pc.format(8,SerialBase::Even,1);
}

int main()
{
    float gx0_cal=0,gy0_cal=0,gz0_cal=0,gx1_cal=0,gy1_cal=0,gz1_cal=0,gy00_cal=0;
    
    serial_setup();
    test1=lsm0.begin(LSM6DS3::G_SCALE_245DPS,LSM6DS3::A_SCALE_8G,LSM6DS3::G_ODR_833,LSM6DS3::A_ODR_833);
    test=lsm1.begin(LSM6DS3::G_SCALE_245DPS,LSM6DS3::A_SCALE_8G,LSM6DS3::G_ODR_833,LSM6DS3::A_ODR_833);
    
    lsm00.begin(LSM6DS3::G_SCALE_245DPS,LSM6DS3::A_SCALE_8G,LSM6DS3::G_ODR_833,LSM6DS3::A_ODR_833);
    //lsm0.setHighPass();
    
    led2.write(1);
    wait(1);
    led2.write(0);
    wait(1);
    
    for(int i=0;i<AVE_COUNT;i++){
        lsm0.readGyro();
        gy0_cal+=lsm0.gy;
        
        lsm1.readGyro();
        gy1_cal+=lsm1.gy;

        lsm00.readGyro();
        gy00_cal += lsm00.gy;
        wait_ms(10);
        //pc.printf("%d %f:%f:%f\r\n",i,lsm0.gy,lsm1.gy,lsm00.gy);
    }
    gy0_cal=gy0_cal/AVE_COUNT;
    gy1_cal=gy1_cal/AVE_COUNT;
    gy00_cal=gy00_cal/AVE_COUNT;   
    //pc.printf("%f:%f:%f\r\n",gy0_cal,gy1_cal,gy00_cal);
    
    led2.write(1);
    
    while(1) {
        lsm0.readAccel();
        lsm0.readGyro_cal(gx0_cal,gy0_cal,gz0_cal);
        a0=sqrt(lsm0.ax*lsm0.ax+lsm0.ay*lsm0.ay+lsm0.az*lsm0.az);


        lsm1.readAccel();
        lsm1.readGyro_cal(gx1_cal,gy1_cal,gz1_cal);
        a1=sqrt(lsm1.ax*lsm1.ax+lsm1.ay*lsm1.ay+lsm1.az*lsm1.az);
        
        
        lsm00.readAccel();
        lsm00.readGyro_cal(0,gy00_cal,0);
        a00=sqrt(lsm00.ax*lsm00.ax+lsm00.ay*lsm00.ay+lsm00.az*lsm00.az);
//        if(a0>1.8f && a0<2.1f){
//            flag|=1;    
//        }
//        else{
//            flag&=254;    
//        }
//        
//        
//        if(a1>1.8f && a1<2.1f){
//            flag|=2;    
//        }
//        else{
//            flag&=253;    
//        }

        if(a0>1.8f && a0<2.2f && a1>1.8f && a1<2.2f ){
            if((lsm0.ax<2.1f && lsm0.ax>1.8f) || (lsm0.ax>-2.1f && lsm0.ax<-1.8f) || (lsm0.ay<2.1f && lsm0.ay>1.8f) || (lsm0.ay>-2.1f && lsm0.ay<-1.8f)){
                flag=0;    
            }
            else{
                flag=3;
            }
        }
        else{
            flag=0;    
        }

        pc.read(buff_in,sizeof(buff_in),rx_event_pointer);

    }
}