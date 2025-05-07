cmake_minimum_required(VERSION 3.15)
project(UnityPlugin LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF) 

set(LIB_TYPE MODULE)
# Set export macro based on platform
if(WIN32)
    add_definitions(-DPLUGIN_EXPORT=__declspec(dllexport))
else()
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE INTERNAL "")
    set(CMAKE_OSX_DEPLOYMENT_TARGET "10.11" CACHE STRING "Minimum OS X deployment version" FORCE)
    if(CMAKE_SYSTEM_NAME STREQUAL "iOS")
        set(LIB_TYPE STATIC)
    endif()
endif()

# Collect sources
file(GLOB_RECURSE SRC_FILES CONFIGURE_DEPENDS src/*.cpp src/*.h)
file(GLOB_RECURSE 3DTI_TOOLKIT CONFIGURE_DEPENDS
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.cpp"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.c"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.h"
)

# Collect your project sources
file(GLOB_RECURSE PROJECT_SRC CONFIGURE_DEPENDS src/*.cpp src/*.h)
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/src" PREFIX "src" FILES ${PROJECT_SRC})

# Collect third-party sources
file(GLOB_RECURSE 3DTI_TOOLKIT "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.cpp" "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.h")
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit" PREFIX "3dti_Toolkit" FILES ${3DTI_TOOLKIT})

file(GLOB_RECURSE 3DTI_MODULES 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/BRIR/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/BRIR/*.h"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/HRTF/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/HRTF/*.h"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/ILD/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/ILD/*.h"
)
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager" PREFIX "3dti_Modules" FILES ${3DTI_MODULES})

file(GLOB_RECURSE SOFA 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src/*.h"
)
foreach(file ${SOFA})
    get_filename_component(fname ${file} NAME)
    if(NOT fname MATCHES "^SOFA")
        list(REMOVE_ITEM SOFA ${file})
    endif()
endforeach()
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src" PREFIX "SOFA" FILES ${SOFA})

file(GLOB_RECURSE NETCDF
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include/*.h"
)
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include" PREFIX "netcdf" FILES ${NETCDF})

# Add library
add_library(UnityPlugin ${LIB_TYPE}
    ${PROJECT_SRC}
    ${3DTI_TOOLKIT}
    ${3DTI_MODULES}
    ${SOFA}
    ${NETCDF}
)

target_include_directories(UnityPlugin PUBLIC 
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/cereal/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/eigen"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include"
)

# Link static library depenencies
file(GLOB STATIC_LIBS "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/lib/macos/*.a")
target_link_libraries(UnityPlugin PRIVATE ${STATIC_LIBS})

# macOS .bundle output
message(STATUS "CMAKE_SYSTEM_NAME: ${CMAKE_SYSTEM_NAME}")
if(APPLE AND CMAKE_SYSTEM_NAME STREQUAL "Darwin")
    set_target_properties(UnityPlugin PROPERTIES
        XCODE_ATTRIBUTE_EXCLUDED_SOURCE_FILE_NAMES "SOFA* nc* HRTFFactory.cpp BRIRFactory.cpp"
        BUNDLE TRUE
        MACOSX_BUNDLE TRUE
        SUFFIX ".bundle"
    )
endif()
