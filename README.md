# VRCImageHelper

VRChat�̃J�����ŎB�����摜�����k���āA

* ���[���h��
* �C���X�^���X�ɂ����v���C���[��
* [VirtualLens2](https://logilabo.booth.pm/items/2280136)��
    * �i��l
    * �œ_����
* [Integral](https://suzufactory.booth.pm/items/4724145)��
    * �i��l
    * �œ_����
    * �I�o�␳
    * (���d�I����) �I������
    * �����Y�̃{�P�̌`��
* (MakerNotes�ɁABase64�ŃG���R�[�h���ꂽJSON�Ƃ���) World Name / World ID / Instance Owner / Permission / Player�̃��X�g

���������ރc�[���ł��B

### �C���X�g�[��

1. �C���X�g�[����[Releases](https://github.com/m-hayabusa/VRCImageHelper/releases/)����_�E�����[�h���A�C���X�g�[������  
(�C���X�g�[������ "You must install or update .NET to run this application." ����n�܂郁�b�Z�[�W���o�Ă����ꍇ�A�u�͂��v���N���b�N����ƕK�v�ȃ��C�u���� (.NET 6.0 Desktop Runtime) �̃_�E�����[�h�y�[�W���J���̂ŁA������C���X�g�[�����Ă��������xVRCImageHelper�̃C���X�g�[����������Ă�������)

2. �C���X�g�[�����ɐݒ��ʂ��o�Ă���̂ŁA�ݒ肷��  
    ![�ݒ���](https://github.com/m-hayabusa/VRCImageHelper/assets/10593623/8b2b56e3-f31e-4017-9c99-e2ce636e8bfd)

    * �ۑ���: �ϊ���̃t�@�C����ۑ�����ꏊ�B���ݒ�̏ꍇ��VRChat���o�͂����t�H���_�ɕۑ������

    * �ۑ��`��
        * �t�@�C����: �ۑ�����Ƃ��̃t�@�C�����B�f�t�H���g�̏ꍇ��VRChat�̏o�͂����t�@�C���Ɠ����ƂȂ�͂�
            |�t�H�[�}�b�g|�u�����e|
            |:-|:-|
            |yyyy|�N|
            |MM|��|
            |dd|��|
            |hh|��|
            |mm|��|
            |ss|�b|
            |fff|�b(�����_��)|
            |XXXX|�摜�̃s�N�Z���� (�c)|
            |YYYY|�摜�̃s�N�Z���� (��)|

        * �`���E�i���E�I�v�V����: PNG / JPEG / WEBP / AVIF���I���ł���
            * PNG: �i���ݒ�ƃI�v�V�����́A���������
            * JPEG: �i���ݒ�́A0���ō��A100���Œ�ƂȂ�B�I�v�V�����͖��������
            * WEBP / AVIF: �i���ݒ�́Affmpeg�ł��ꂼ��̃G���R�[�_��
                |�G���R�[�_|����|�ō��i��|�ō����k|
                |:-|:-|:-|:-|
                |libwebp|`-quality`|0|100|
                |libaom-av1|`-crf`|0|63|
                |libsvtav1|`-crf`|0|63|
                |av1_qsv|`-q`|?|?|
                |av1_nvenc|`-cq`|1|51|
                |av1_amf|`-qp_i`|0|255|

                CPU��libwebp�Alibaom-av1��libsvtav1�𗘗p����ꍇ�ƁAIntel Arc��av1_qsv�𗘗p�����ꍇ�̂݊m�F (NvEnc / AMF�̏ꍇ�̓���ɂ��Ă͈�،��؂ł��Ă��Ȃ��̂ŁA�������p�����[�^�̎w��Ȃǂ����[�����Ă�������](https://github.com/m-hayabusa/VRCImageHelper/issues/new))  
                �I�v�V�����́Affmpeg�ɒǉ��œn����������͂ł��� ���Ƃ���libwebp�� `-lossless 1` �Ȃ�
    * �ۑ��`��(����)  
        �摜�ɃA���t�@�`���l�����܂܂��ꍇ�̌`���w��ŁA����ȊO�͏�L �ۑ��`�� �Ɠ����B������:
        * JPEG: ��Ή�
        * AVIF: ���߂ɑΉ����Ȃ�AV1�G���R�[�_������ (�茳�̊��ł�libaom-av1���������x�������ł��Ȃ��悤������)
3. (����VRChat Exif Writer���C���X�g�[���������Ƃ�����A�폜���Ă��Ȃ��ꍇ) VRChat Exif Writer���폜���邱�Ƃɂ��Ċm�F���b�Z�[�W���o��̂ŁA���ɗ��R���Ȃ���΁A���̂܂܍폜���Ă�������

4. �X�^�[�g���j���[����VRCImageHelper���N�����A�^�X�N�o�[�ɏo�Ă���A�C�R�� <img style="height:1em" src="https://github.com/m-hayabusa/VRCImageHelper/raw/master/VRCImageHelper/icon.ico"> ���E�N���b�N�A�����N���̃`�F�b�N������ƁA���񂩂�PC�Ƀ��O�C�������ۂɎ����ŋN������悤�ɂȂ�

### �A���C���X�g�[��

Windows�� �ݒ�/�A�v��/�C���X�g�[������Ă���A�v�� ����A���C���X�g�[���ł��� (�����ɐݒ�t�@�C�����폜�����)

### ExifTool��ffmpeg�ɂ���
������exiftool.exe��ffmpeg.exe�𗘗p���邪�A���ϐ� PATH �ɐݒ肳�ꂽ�f�B���N�g�����Ɍ�����Ȃ���΁A�����Ń_�E�����[�h����̂ŁA������肪�Ȃ���Ηp�ӂ���K�v�͂Ȃ��B�����Ń_�E�����[�h�������̂�:
* ExifTool: https://sourceforge.net/projects/exiftool/files/latest/download
* ffmpeg: https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip