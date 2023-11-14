################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/jobarg_server/jatrapper/jatrapauth.c \
../src/jobarg_server/jatrapper/jatrapjoblogput.c \
../src/jobarg_server/jatrapper/jatrapjobnetrun.c \
../src/jobarg_server/jatrapper/jatrapjobrelease.c \
../src/jobarg_server/jatrapper/jatrapjobresult.c \
../src/jobarg_server/jatrapper/jatrapkind.c \
../src/jobarg_server/jatrapper/jatrapper.c 

OBJS += \
./src/jobarg_server/jatrapper/jatrapauth.o \
./src/jobarg_server/jatrapper/jatrapjoblogput.o \
./src/jobarg_server/jatrapper/jatrapjobnetrun.o \
./src/jobarg_server/jatrapper/jatrapjobrelease.o \
./src/jobarg_server/jatrapper/jatrapjobresult.o \
./src/jobarg_server/jatrapper/jatrapkind.o \
./src/jobarg_server/jatrapper/jatrapper.o 

C_DEPS += \
./src/jobarg_server/jatrapper/jatrapauth.d \
./src/jobarg_server/jatrapper/jatrapjoblogput.d \
./src/jobarg_server/jatrapper/jatrapjobnetrun.d \
./src/jobarg_server/jatrapper/jatrapjobrelease.d \
./src/jobarg_server/jatrapper/jatrapjobresult.d \
./src/jobarg_server/jatrapper/jatrapkind.d \
./src/jobarg_server/jatrapper/jatrapper.d 


# Each subdirectory must supply rules for building sources it contributes
src/jobarg_server/jatrapper/%.o: ../src/jobarg_server/jatrapper/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


