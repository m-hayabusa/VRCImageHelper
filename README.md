# VRCImageHelper

VRChat�̃J�����ŎB�����摜�����k���āA

* ���[���h��
* �C���X�^���X�ɂ����v���C���[��
* [VirtualLens2](https://logilabo.booth.pm/items/2280136)��
    * �i��l
    * �œ_����
    * �I�o�␳
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
            | �t�H�[�}�b�g | �u�����e | �� |
            |:-|:-|:-|
            | `yyyy` | �N | `2025` |
            | `MM` | �� | `04` |
            | `dd` | �� | `05` |
            | `hh` | �� | `14` |
            | `mm` | �� | `19` |
            | `ss` | �b | `53` |
            | `fff` | �b(�����_��) | `375` |
            | `XXXX` | �摜�̃s�N�Z���� (�c) | `3840` |
            | `YYYY` | �摜�̃s�N�Z���� (��) | `2160` |
            | `%CAMERA%` | �J�����̎�� | `VRCCamera` |
            | `%WORLD_ID%` | ���[���hID | `wrld_3208d019-7310-4c35-b12e-e4278c2689c7` |
            | `%INSTANCE_ID%` | �C���X�^���X�ԍ� | `99424` |
            | `%WORLD%` | ���[���h�� | `nS�^TownScaper` |
            | `%INSTANCE_TYPE%` | �C���X�^���X�̎�� | `Friends+` |
            | `%OWNER_ID%` | �C���X�^���X�I�[�i�[��ID | `usr_cbced732-f21a-46cd-a6a6-61990bceea14` |

            �Ⴆ�΁A`yyyy-MM\%WORLD_ID%-%WORLD%\%OWNER_ID%\%INSTANCE_TYPE%\%INSTANCE_ID%\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY_%CAMERA%.jpeg` �Ǝw�肵���Ƃ��A
            �ۑ���t�H���_�� `D:\Pictures\VRChat` �Ȃ�A`D:\Pictures\VRChat\2025-04\wrld_3208d019-7310-4c35-b12e-e4278c2689c7-nS�^TownScaper\usr_cbced732-f21a-46cd-a6a6-61990bceea14\Friends+\99424\VRChat_2025-04-05_14-19-53.375_3840x2160_VRCCamera.jpeg` �ɕۑ������

        * �`���E�i���E�I�v�V����: PNG / JPEG / WEBP / AVIF���I���ł���
            * PNG: �i���ݒ�ƃI�v�V�����́A���������
            * JPEG: �i���ݒ�́A0���ō��A100���Œ�ƂȂ�B�I�v�V�����͖��������
            * WEBP / AVIF: �i���ݒ�́Affmpeg�ł��ꂼ��̃G���R�[�_��
                | �G���R�[�_ | ���� | �ō��i�� | �ō����k |
                |:-|:-|:-|:-|
                | `libwebp` | `-quality` | `0` | `100` |
                | `libaom-av1` | `-crf` | `0` | `63` |
                | `libsvtav1` | `-crf` | `0` | `63` |
                | `av1_qsv` | `-q` | `?` | `?` |
                | `av1_nvenc` | `-cq` | `1` | `51` |
                | `av1_amf` | `-qp_i` | `0` | `255` |

                CPU��libwebp�Alibaom-av1��libsvtav1�𗘗p����ꍇ�ƁAIntel Arc A770��av1_qsv�𗘗p�����ꍇ�AAMD Radeon 780M��av1_amf�𗘗p�����ꍇ�ɂ��ē�����m�F  
                (NvEnc�ł̓���͌��؂��Ă��܂��񂪁A�f�t�H���g�Ŏw�肵�Ă���I�v�V������ `--pix-fmt yuv420p` �̉e���ŁA�F��񂪊Ԉ�����鋓���ɂȂ�͂��ł� (https://github.com/m-hayabusa/VRCImageHelper/issues/40))  
                �I�v�V�����́Affmpeg�ɒǉ��œn����������͂ł��� ���Ƃ���libwebp�� `-lossless 1` �Ȃ�
    * �ۑ��`��(����)  
        �摜�ɃA���t�@�`���l�����܂܂��ꍇ�̌`���w��ŁA����ȊO�͏�L �ۑ��`�� �Ɠ����B������:
        * JPEG: ��Ή�
        * AVIF: ���߂ɑΉ����Ȃ�AV1�G���R�[�_������ (�茳�̊��ł�libaom-av1���������x�������ł��Ȃ��悤������)
3. (����VRChat Exif Writer���C���X�g�[���������Ƃ�����A�폜���Ă��Ȃ��ꍇ) VRChat Exif Writer���폜���邱�Ƃɂ��Ċm�F���b�Z�[�W���o��̂ŁA���ɗ��R���Ȃ���΁A���̂܂܍폜���Ă�������

4. �X�^�[�g���j���[����VRCImageHelper���N�����A�^�X�N�o�[�ɏo�Ă���A�C�R�� <img style="height:1em" src="https://github.com/m-hayabusa/VRCImageHelper/raw/master/VRCImageHelper/icon.ico"> ���E�N���b�N�A�����N���̃`�F�b�N������ƁA���񂩂�PC�Ƀ��O�C�������ۂɎ����ŋN������悤�ɂȂ�

### �uVRChat�̃��O�o�͂��I�t�ɂȂ��Ă��܂��񂩁H�v����n�܂�ʒm���\�����ꂽ�ꍇ

VRChat�̐ݒ肩��A�uLogging�v��L���ɂ���K�v������\��������̂ŁA�m�F���K�v
![image](https://github.com/m-hayabusa/VRCImageHelper/assets/10593623/b4a22571-bf88-4353-80e3-908323dd2470)

### �A���C���X�g�[��

Windows�� �ݒ�/�A�v��/�C���X�g�[������Ă���A�v�� ����A���C���X�g�[���ł��� (�����ɐݒ�t�@�C�����폜�����)

### ExifTool��ffmpeg�ɂ���
������exiftool.exe��ffmpeg.exe�𗘗p���邪�A���ϐ� PATH �ɐݒ肳�ꂽ�f�B���N�g�����Ɍ�����Ȃ���΁A�����Ń_�E�����[�h����̂ŁA������肪�Ȃ���Ηp�ӂ���K�v�͂Ȃ��B�����Ń_�E�����[�h�������̂�:
* ExifTool: https://sourceforge.net/projects/exiftool/files/latest/download
* ffmpeg: https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip