################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/jobarg_server/jajobnet/jajobnet.c \
../src/jobarg_server/jajobnet/jajobnetkill.c \
../src/jobarg_server/jajobnet/jajobnetready.c \
../src/jobarg_server/jajobnet/jajobnetrun.c \
../src/jobarg_server/jajobnet/jajobnetsummaryready.c 

OBJS += \
./src/jobarg_server/jajobnet/jajobnet.o \
./src/jobarg_server/jajobnet/jajobnetkill.o \
./src/jobarg_server/jajobnet/jajobnetready.o \
./src/jobarg_server/jajobnet/jajobnetrun.o \
./src/jobarg_server/jajobnet/jajobnetsummaryready.o 

C_DEPS += \
./src/jobarg_server/jajobnet/jajobnet.d \
./src/jobarg_server/jajobnet/jajobnetkill.d \
./src/jobarg_server/jajobnet/jajobnetready.d \
./src/jobarg_server/jajobnet/jajobnetrun.d \
./src/jobarg_server/jajobnet/jajobnetsummaryready.d 


# Each subdirectory must supply rules for building sources it contributes
src/jobarg_server/jajobnet/%.o: ../src/jobarg_server/jajobnet/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


