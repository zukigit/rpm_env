################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/jobarg_server/jajob/jajob.c \
../src/jobarg_server/jajob/jajobiconextjob.c \
../src/jobarg_server/jajob/jajobiconjob.c \
../src/jobarg_server/jajob/jajobiconless.c \
../src/jobarg_server/jajob/jajobiconreboot.c 

OBJS += \
./src/jobarg_server/jajob/jajob.o \
./src/jobarg_server/jajob/jajobiconextjob.o \
./src/jobarg_server/jajob/jajobiconjob.o \
./src/jobarg_server/jajob/jajobiconless.o \
./src/jobarg_server/jajob/jajobiconreboot.o 

C_DEPS += \
./src/jobarg_server/jajob/jajob.d \
./src/jobarg_server/jajob/jajobiconextjob.d \
./src/jobarg_server/jajob/jajobiconjob.d \
./src/jobarg_server/jajob/jajobiconless.d \
./src/jobarg_server/jajob/jajobiconreboot.d 


# Each subdirectory must supply rules for building sources it contributes
src/jobarg_server/jajob/%.o: ../src/jobarg_server/jajob/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


