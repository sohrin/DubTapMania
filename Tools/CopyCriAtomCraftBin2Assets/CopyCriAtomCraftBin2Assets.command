# chmod +x CopyCriAtomCraftBin2Assets.command

cd `dirname $0`

rm -rf ../../Assets/StreamingAssets/*.acf
rm -rf ../../Assets/StreamingAssets/*.acf.meta
rm -rf ../../Assets/StreamingAssets/*.acb
rm -rf ../../Assets/StreamingAssets/*.acb.meta

cp -a ../../CriAtomCraftProject/DubTapMania/Public/Assets/StreamingAssets/* ../../Assets/StreamingAssets/
cp -a ../../CriAtomCraftProject/DubTapMania/Public/DubTapMania_acf.cs ../../Assets/Scripts/